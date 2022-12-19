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
