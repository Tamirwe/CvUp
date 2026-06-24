import axios from "axios";
import { IIdName } from "../models/AuthModels";
import { ICand } from "../models/GeneralModels";

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

export const sortCandList = (isDesc: boolean, list: ICand[]) => {
  const sorted = list.slice();
  return isDesc
    ? sorted.sort((a, b) => (a.cvSent > b.cvSent ? 1 : b.cvSent > a.cvSent ? -1 : 0))
    : sorted.sort((a, b) => (a.cvSent < b.cvSent ? 1 : b.cvSent < a.cvSent ? -1 : 0));
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
    `https://translation.googleapis.com/language/translate/v2?key=`,
    { q: text, target: "en", source: "iw" }
  );
  let translation = res.data.data.translations[0].translatedText;
  return translation;
};
