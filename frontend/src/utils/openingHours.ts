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
