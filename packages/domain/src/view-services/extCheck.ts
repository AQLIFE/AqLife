import { defaultFilePolicy } from "@aqlife/domain";

const IMAGE_EXTENSIONS = ():string[] =>defaultFilePolicy.allowedUpload.filter((item) => item !== '.md') // 过滤掉不需要的 .md，只留下图片
const MARKDOWN_EXTENSIONS = ['.md','.markdown']
const EXTENSION_REGEXP = /^\.[a-zA-Z0-9]+$/;
/**
 * 判断是否为图片类型
 * @param input 可以是 File 对象，也可以是 string 后缀（如 '.jpg' 或 'image.png'）
 */
function matchFileType(input: File | string): string | null {
  let ext = '';

  if (typeof input === 'string') {
    // 兼容处理：用户可能传了 '.jpg'，也可能传了 'avatar.jpg'
    ext = input.includes('.') ? input.substring(input.lastIndexOf('.')) : `.${input}`;
  } else if (input instanceof File && input.name) {
    // 传 File 对象时，从全名中提取后缀
    ext = input.name.substring(input.name.lastIndexOf('.'));
  } else {
    return null;
  }

  // 统一转小写，并用正则做最后防御
  ext = ext.toLowerCase();
  return EXTENSION_REGEXP.test(ext) ? ext : null;
}

/**
 * 判断是否为图片类型
 */
export function isImageType(input: File | string): boolean {
  const ext = matchFileType(input);
  return ext !== null && IMAGE_EXTENSIONS().includes(ext);
}

/**
 * 判断是否为 Markdown 类型
 */
export function isMdType(input: File | string): boolean {
  const ext = matchFileType(input);
  return ext !== null && MARKDOWN_EXTENSIONS.includes(ext);
}