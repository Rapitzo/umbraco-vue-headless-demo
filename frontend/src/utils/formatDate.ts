const formatter = new Intl.DateTimeFormat('en-GB', { day: 'numeric', month: 'long', year: 'numeric' })

/** "2026-09-11T00:00:00" -> "11 September 2026" */
export function formatDate(value: string): string {
  return formatter.format(new Date(value))
}
