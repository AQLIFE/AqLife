export type ValidationResult =
  | { ok: true }
  | { ok: false; message: string }

export function validationOk(): ValidationResult {
  return { ok: true }
}

export function validationFail(message: string): ValidationResult {
  return { ok: false, message }
}

export function firstFailure(results: ValidationResult[]): ValidationResult {
  return results.find((item) => !item.ok) ?? validationOk()
}
