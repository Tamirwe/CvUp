import axios from "axios";
import { IIdName } from "../models/AuthModels";

type EnumType = { [s: number]: string };

export const enumToArrays = (en: EnumType) => {
  const keysValuesArr = Object.keys(en);
  const halfArr = keysValuesArr.length / 2;
  const enumArr: IIdName[] = [];

  for (let i = 0; i < halfArr; i++) {
    enumArr.push({
      id: parseInt(keysValuesArr[i]),
      name: keysValuesArr[i + halfArr],
    });
  }

  return enumArr;
};

export const delay = (time: number) => {
  return new Promise((resolve) => setTimeout(resolve, time));
};
// await this.delay(1000);

export const copyToClipBoard = async (copyMe: string) => {
  try {
    await navigator.clipboard.writeText(copyMe);
    return true;
  } catch (err) {
    return false;
  }
};

export const numArrRemoveItem = (id: number, numArr?: number[]) => {
  if (numArr) {
    let index = numArr.indexOf(id);

    if (index > -1) {
      numArr.splice(index, 1);
    }
  }
};

export const translate = async (text: string) => {
  let res = await axios.post(
    `https://translation.googleapis.com/language/translate/v2?key=AIzaSyBWNywjQE_XJvKq8XEJ50_tetaoDLI9O4M`,
    { q: text, target: "en", source: "iw" }
  );
  let translation = res.data.data.translations[0].translatedText;
  return translation;
};
