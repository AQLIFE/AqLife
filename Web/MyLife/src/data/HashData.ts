class SHA256{
    constructor(hashString:string){
        if(!/^[a-fA-F0-9]{64}$/.test(hashString)){
            throw new Error("Invalid SHA256 hash string");
        }
        this.hashString = hashString;
    }
    private hashString:string;

    toString():string{
        return this.hashString;
    }
}


export {type SHA256};