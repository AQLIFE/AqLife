interface IReason {
    status: boolean
    content: 'PASS'|'Length Error'|'Format Error'|'Not match'|'default'
}

type Algorithm = 'SHA-256'|'SHA-384'|'SHA-512'


/**
 * @hideconstructor
 * @description 哈希处理工具类
 */
class Hash {
    /**
     * 
     * @param input 待处理的源信息
     * @param algorithm 哈希算法，可选 @default SHA-256
     * @returns 16 进制的字符串
     */
    static async  generate(input: string, algorithm:Algorithm = 'SHA-256'):Promise<HashSHA> {
        const encoder = new TextEncoder()
        const data = encoder.encode(input)
        const hashBuffer = await crypto.subtle.digest(algorithm, data)
        const hashArray = Array.from(new Uint8Array(hashBuffer))
        return hashArray.map(b => b.toString(16).padStart(2, '0')).join('') as HashSHA
    }

    /**
     * @async
     * @param input 普通文本 或者待确认的 hash 序列
     * @param hash 被对比的 hash 序列
     * @param algorithm 哈希算法，可选 @default SHA-256
     * @returns 验证结果 @see IReason
     */
    static async verify(input: string, hash: string = '', algorithm:Algorithm = 'SHA-256'):Promise<IReason> {
        const source = hash!='' && hash!=null ? await this.generate(input, algorithm): input;
        if (source.length=== SHA256Len || source.length=== SHA384Len || source.length=== SHA512Len)
            this.Reason = source === hash ?  {status:true, content:'PASS'} : {status:false, content:'Length Error'};
        else
            this.Reason = {status:false, content:'Format Error'};
        return this.Reason;
    }


    static random(algorithm:Algorithm = 'SHA-256'):Promise<HashSHA>{
        const value = new Uint8Array(10)
        window.crypto.getRandomValues(value)
        const str = value.toString()
        console.log(str)
        return this.generate(str,algorithm)
    }

    private static  Reason:IReason = {
        status: false,
        content:'default'
    }
}

const SHA256Len= 64;//256/4;
const SHA384Len= 96;//384/4;
const SHA512Len= 128;//512/4;

type SHA256 = string & {length:typeof SHA256Len}
type SHA384 = string & {length:typeof SHA384Len}
type SHA512 = string & {length:typeof SHA512Len}

type HashSHA = SHA256|SHA384|SHA512 
const HashSHADefault:HashSHA = 'BA7816BF8F01CFEA414140DE5DAE2223B00361A396177A9CB410FF61F20015AD' as SHA256
export  {type SHA256, type SHA384,type SHA512,type HashSHA ,Hash,HashSHADefault }