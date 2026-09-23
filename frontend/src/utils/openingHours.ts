export interface OpeningHoursRow {
  days: string
  hours: string
}

/** Editors write one line per day range: "Monday to Friday, 07:00 to 18:00". */
export function parseOpeningHours(value: string | null | undefined): OpeningHoursRow[] {
  return (value ?? '')
    .split('\n')
    .map((line) => line.trim())
    .filter(Boolean)
    .map((line) => {
      const comma = line.indexOf(',')
      return comma === -1
        ? { days: line, hours: '' }
        : { days: line.slice(0, comma).trim(), hours: line.slice(comma + 1).trim() }
    })
}

const DAYS = ['sunday', 'monday', 'tuesday', 'wednesday', 'thursday', 'friday', 'saturday']

/**
 * The hours that apply on a given date, or null if no row matches.
 * Understands single days ("Saturday") and ranges ("Monday to Friday").
 */
export function hoursForDate(rows: OpeningHoursRow[], date: Date): string | null {
  const today = date.getDay()
  const row = rows.find(({ days }) => {
    const [from, to = from] = days.toLowerCase().split(/\s+to\s+/).map((day) => DAYS.indexOf(day.trim()))
    if (from === undefined || from < 0 || to < 0) return false
    return from <= to ? today >= from && today <= to : today >= from || today <= to
  })
  return row?.hours ?? null
}
