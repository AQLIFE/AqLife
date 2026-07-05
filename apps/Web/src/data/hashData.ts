interface Reason {
  status: boolean
  content: 'PASS' | 'Length Error' | 'Format Error' | 'Not match' | 'default'
}

type Algorithm = 'SHA-256' | 'SHA-384' | 'SHA-512'

class Hash {
  static async generate(input: string, algorithm: Algorithm = 'SHA-256'): Promise<HashSHA> {
    const encoder = new TextEncoder()
    const data = encoder.encode(input)
    const hashBuffer = await crypto.subtle.digest(algorithm, data)
    const hashArray = Array.from(new Uint8Array(hashBuffer))
    return hashArray.map((b) => b.toString(16).padStart(2, '0')).join('') as HashSHA
  }

  static async verify(
    input: string,
    hash = '',
    algorithm: Algorithm = 'SHA-256',
  ): Promise<Reason> {
    const source = hash !== '' && hash != null ? await this.generate(input, algorithm) : input
    if (source.length === SHA256Len || source.length === SHA384Len || source.length === SHA512Len) {
      return source === hash ? { status: true, content: 'PASS' } : { status: false, content: 'Length Error' }
    }
    return { status: false, content: 'Format Error' }
  }

  static random(algorithm: Algorithm = 'SHA-256'): Promise<HashSHA> {
    const value = new Uint8Array(10)
    window.crypto.getRandomValues(value)
    return this.generate(value.toString(), algorithm)
  }
}

const SHA256Len = 64
const SHA384Len = 96
const SHA512Len = 128

type SHA256 = string & { length: typeof SHA256Len }
type SHA384 = string & { length: typeof SHA384Len }
type SHA512 = string & { length: typeof SHA512Len }

type HashSHA = SHA256 | SHA384 | SHA512

export const hashShaDefault: HashSHA =
  'BA7816BF8F01CFEA414140DE5DAE2223B00361A396177A9CB410FF61F20015AD' as SHA256

export { type SHA256, type SHA384, type SHA512, type HashSHA, Hash }
