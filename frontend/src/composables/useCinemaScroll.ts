import { onBeforeUnmount, onMounted, ref, type Ref } from 'vue'

const clamp = (v: number, min = 0, max = 1) => Math.min(max, Math.max(min, v))
const lerp = (a: number, b: number, t: number) => a + (b - a) * t

function smoothstep(e0: number, e1: number, v: number): number {
  const x = clamp((v - e0) / (e1 - e0))
  return x * x * (3 - 2 * x)
}

/** Fades in between a and b, out between c and d. `active` is the product of both. */
function segment(s: number, a: number, b: number, c: number, d: number) {
  const enter = smoothstep(a, b, s)
  const exit = smoothstep(c, d, s)
  return { enter, exit, active: enter * (1 - exit) }
}

/** Horizontal screen position of the card slider's left edge, in px. */
const SLIDER_SCREEN_LEFT = 48

/**
 * Drives the cinema scroll: reads how far the tall section has been scrolled, eases it
 * (and the pointer) toward the target, and writes the choreography as custom properties on
 * the section. All numbers are pixel offsets into the scroll, so the timeline is fixed-length:
 * 3100px, or 3950px with the closing bake board scene (the section is 100vh taller). With
 * reduced motion the values snap and the pointer parallax is off.
 */
export function useCinemaScroll(
  section: Ref<HTMLElement | null>,
  stage: Ref<HTMLElement | null>,
  withBoard: boolean,
) {
  const end = withBoard ? 3950 : 3100
  const controlsReady = ref(false)

  const reduceMotion = window.matchMedia('(prefers-reduced-motion: reduce)')
  let targetMouseX = 0
  let targetMouseY = 0
  let mouseX = 0
  let mouseY = 0
  let targetScroll = 0
  let smoothScroll = 0
  let initialized = false
  let frame = 0

  function scrollDistance(el: HTMLElement): number {
    return clamp(-el.getBoundingClientRect().top, 0, el.offsetHeight - window.innerHeight)
  }

  function update() {
    frame = 0
    const el = section.value
    const stageEl = stage.value
    if (!el || !stageEl) return

    targetScroll = scrollDistance(el)
    const snap = !initialized || reduceMotion.matches
    smoothScroll = snap ? targetScroll : lerp(smoothScroll, targetScroll, 0.14)
    initialized = true
    if (Math.abs(smoothScroll - targetScroll) < 0.08) smoothScroll = targetScroll

    const px = reduceMotion.matches ? 0 : 1
    mouseX = lerp(mouseX, targetMouseX * px, 0.12)
    mouseY = lerp(mouseY, targetMouseY * px, 0.12)

    const s = smoothScroll
    const frame2 = segment(s, 560, 900, 1300, 1620)
    const frame3 = segment(s, 1760, 2140, 2540, 2700)
    const progress = clamp(s / 2700)
    const introExit = smoothstep(90, 650, s)
    // The slider starts arriving behind the café panel (blurred, as background), so that
    // scene doesn't hold on its own for long.
    const cardsEnter = smoothstep(1950, 2750, s)
    const controlsEnter = smoothstep(2600, 2900, s)
    // Board scene, as one continuous move: the slider carries on out of frame to the left while
    // the field blurs, the heading rises and the bakes come in right behind it from the right,
    // so there is never a frame with only the background.
    const cardsOut = withBoard ? smoothstep(3000, 3500, s) : 0
    const boardEnter = withBoard ? smoothstep(3050, 3450, s) : 0
    const bakesEnter = withBoard ? smoothstep(3100, 3700, s) : 0
    const blurActive = clamp(frame2.active + frame3.active)
    // The board scene blurs and tints the field behind it, like the story panels do.
    const dim = Math.max(blurActive, boardEnter)
    const splitDrift = Math.pow(frame2.enter, 1.5)
    const backScale = 0.76 + progress * 0.2 + frame2.enter * 0.18 + frame3.enter * 0.16
    const heroY = progress * -74
    const heroScale = progress * 0.23

    // The slider lives inside the scaled back stack (so it sits between two background
    // layers). Solve its local position so the cards land at a fixed screen position.
    const h = stageEl.clientHeight
    const w = stageEl.clientWidth
    const cardsScreenTop = clamp(h * 0.19, 112, 220) - 50
    const stackWidth = w * 1.06
    const stackLeft = -w * 0.03
    const cardsTop = h - (h - cardsScreenTop) / backScale
    const cardsLeft = stackWidth / 2 + (SLIDER_SCREEN_LEFT - (stackLeft + stackWidth / 2)) / backScale

    const vars: Record<string, string> = {
      '--back-opacity': (1 - frame2.active * 0.06).toFixed(4),
      '--back-x': `${(mouseX * -12).toFixed(2)}px`,
      '--back-y': `${(mouseY * -4).toFixed(2)}px`,
      '--back-scale': backScale.toFixed(4),
      '--back-layer-y': `${(20 - progress * 8).toFixed(3)}vh`,
      '--blur-px': `${(dim * 14).toFixed(2)}px`,
      '--back-brightness': (1 - dim * 0.255).toFixed(4),
      '--back-layer-blur-px': `${(Math.max(frame2.active, boardEnter) * 12).toFixed(2)}px`,
      '--back-layer-brightness': (1 - frame2.active * 0.255 - frame3.active * 0.06).toFixed(4),
      '--back-layer-saturation': (1 + frame3.active * 0.18).toFixed(4),
      '--shade-z': boardEnter > 0.02 ? '7' : frame2.active > 0.02 ? '2' : '0',
      '--shade-top': (dim * 0.465).toFixed(4),
      '--shade-mid': (dim * 0.42).toFixed(4),
      '--shade-bottom': (dim * 0.51).toFixed(4),

      '--title-y': `${(introExit * -210).toFixed(2)}px`,
      '--title-scale': (1 - introExit * 0.08).toFixed(4),
      '--title-opacity': (1 - introExit).toFixed(4),

      '--fg-x': `calc(-50% + ${(mouseX * 18).toFixed(2)}px)`,
      '--fg-y': `${(mouseY * 8 + heroY - frame2.exit * 760).toFixed(2)}px`,
      '--fg-bottom': `${(5 - frame2.enter * 13).toFixed(3)}vh`,
      '--fg-width': `${(52 + frame2.enter * 40).toFixed(3)}vw`,
      '--fg-scale': (1.02 + heroScale + frame2.exit * 0.46).toFixed(4),
      // Gone by the time the close-up is fully in, so it never shows through it as that fades
      // out again (and never covers the cards later).
      '--fg-opacity': (1 - frame2.enter * frame2.enter).toFixed(4),

      '--split-left-x': `calc(${(-splitDrift * 46).toFixed(3)}vw + ${(mouseX * 22).toFixed(2)}px)`,
      '--split-right-x': `calc(${(splitDrift * 46).toFixed(3)}vw + ${(mouseX * 22).toFixed(2)}px)`,
      '--split-y': `${(mouseY * 10 + heroY - splitDrift * 180).toFixed(2)}px`,
      '--split-scale': (1 + heroScale + frame2.enter * 0.74).toFixed(4),
      '--split-opacity': (1 - frame3.enter).toFixed(4),

      '--close-opacity': (frame2.active * (1 - frame3.enter)).toFixed(4),
      '--close-x': `calc(-50% + ${(mouseX * 10).toFixed(2)}px)`,
      '--close-y': `calc(-50% + ${(mouseY * 8 - frame2.exit * 150).toFixed(2)}px)`,
      '--close-scale': (1.06 + frame2.enter * 0.08 + frame2.exit * 0.08).toFixed(4),

      '--intro-y': `${(introExit * 90).toFixed(2)}px`,
      '--intro-opacity': (1 - introExit).toFixed(4),
      '--panel1-opacity': (frame2.active * (1 - frame2.exit)).toFixed(4),
      '--panel1-y': `calc(-50% + ${(-frame2.exit * 86 + (1 - frame2.enter) * 58).toFixed(2)}px)`,
      '--panel2-opacity': (frame3.active * (1 - frame3.exit)).toFixed(4),
      '--panel2-y': `calc(-50% + ${(-frame3.exit * 86 + (1 - frame3.enter) * 58).toFixed(2)}px)`,
      // Hidden, not just transparent, so the panel's link is out of the tab order while unseen.
      '--panel2-visibility': frame3.active > 0.01 ? 'visible' : 'hidden',

      '--cards-visibility': cardsEnter > 0.01 && cardsOut < 0.99 ? 'visible' : 'hidden',
      '--cards-enter-x': `${((1 - cardsEnter) * 140 - cardsOut * 130).toFixed(3)}vw`,
      '--cards-blur': `${(frame3.active * 8).toFixed(2)}px`,
      // Faint while the café panel is still up, so its text keeps its contrast.
      '--cards-opacity': ((1 - smoothstep(0.45, 1, cardsOut)) * (1 - frame3.active * 0.65)).toFixed(4),
      '--cards-scale': (1 / backScale).toFixed(4),
      '--cards-top': `${cardsTop.toFixed(2)}px`,
      '--cards-left': `${cardsLeft.toFixed(2)}px`,
      '--cards-screen-top': `${cardsScreenTop.toFixed(2)}px`,
      '--controls-opacity': (controlsEnter * (1 - cardsOut)).toFixed(4),
      '--controls-visibility': controlsEnter > 0.01 && cardsOut < 0.99 ? 'visible' : 'hidden',
      '--board-opacity': boardEnter.toFixed(4),
      '--board-y': `${((1 - boardEnter) * 80).toFixed(2)}px`,
      '--board-visibility': boardEnter > 0.01 ? 'visible' : 'hidden',
      '--bakes-progress': bakesEnter.toFixed(4),
      '--outro': smoothstep(end - 220, end, s).toFixed(4),
    }
    for (const [name, value] of Object.entries(vars)) el.style.setProperty(name, value)
    controlsReady.value = controlsEnter > 0.98 && cardsOut < 0.02

    const settling =
      Math.abs(smoothScroll - targetScroll) > 0.08 ||
      Math.abs(mouseX - targetMouseX * px) > 0.001 ||
      Math.abs(mouseY - targetMouseY * px) > 0.001
    if (settling) requestTick()
  }

  function requestTick() {
    if (!frame) frame = requestAnimationFrame(update)
  }

  function onPointerMove(event: PointerEvent) {
    targetMouseX = event.clientX / window.innerWidth - 0.5
    targetMouseY = event.clientY / window.innerHeight - 0.5
    requestTick()
  }

  onMounted(() => {
    window.addEventListener('scroll', requestTick, { passive: true })
    window.addEventListener('resize', requestTick)
    window.addEventListener('pointermove', onPointerMove, { passive: true })
    requestTick()
  })

  onBeforeUnmount(() => {
    window.removeEventListener('scroll', requestTick)
    window.removeEventListener('resize', requestTick)
    window.removeEventListener('pointermove', onPointerMove)
    cancelAnimationFrame(frame)
  })

  return { controlsReady }
}
