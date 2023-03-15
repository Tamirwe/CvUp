import { TextValidateTypeEnum } from "../models/GeneralEnums";

/* eslint-disable no-useless-escape */
export const validateTxt = (
  txt: string,
  typeValidation: TextValidateTypeEnum[]
) => {
  for (let i = 0; i < typeValidation.length; i++) {
    switch (typeValidation[i]) {
      case TextValidateTypeEnum.notEmpty:
        if (txt.trim().length < 1) {
          return "Required Field";
        }

        break;
      case TextValidateTypeEnum.onlyLetters:
        if (!/^[\p{L}\s'`-]*$/u.test(txt.trim())) {
          return "Please use letters only";
        }
        break;
      case TextValidateTypeEnum.startWithTwoLetters:
        if (!/^.*[\p{L}\s]{2,}.*$/u.test(txt.trim())) {
          return "Must start with at least two letter.";
        }
        break;
      case TextValidateTypeEnum.twoCharsMin:
        if (txt.trim().length < 2) {
          return "Field is too short";
        }
        break;
      default:
        return "";
    }
  }

  return "";
};

export const validteEmail = (
  email: string,
  typeValidation: TextValidateTypeEnum[]
) => {
  for (let i = 0; i < typeValidation.length; i++) {
    switch (typeValidation[i]) {
      case TextValidateTypeEnum.notEmpty:
        if (email.trim().length < 1) {
          return "Required Field";
        }

        break;
      case TextValidateTypeEnum.emailValid:
        if (!isEmailValid(email)) {
          return "Must be a valid email";
        }
        break;
      default:
        return "";
    }
  }

  return "";
};

export const validtePhone = (
  phone: string,
  typeValidation: TextValidateTypeEnum[]
) => {
  for (let i = 0; i < typeValidation.length; i++) {
    switch (typeValidation[i]) {
      case TextValidateTypeEnum.notEmpty:
        if (phone.trim().length < 1) {
          return "Required Field";
        }

        break;
      case TextValidateTypeEnum.phoneValid:
        if (!isPhoneValid(phone)) {
          return "Must be a valid phone";
        }
        break;
      default:
        return "";
    }
  }

  return "";
};

export const validateSelect = (
  num: number,
  typeValidation: TextValidateTypeEnum[]
) => {
  for (let i = 0; i < typeValidation.length; i++) {
    switch (typeValidation[i]) {
      case TextValidateTypeEnum.notSelected:
        if (num === 0) {
          return " Value must be selected";
        }
        break;
      default:
        return "";
    }
  }

  return "";
};
/***************************************************** */
export const textFieldValidte = (
  txt: string,
  notEmpty: boolean = true,
  twoCharsMin: boolean = true,
  startWithLetters: boolean = false,
  onlyLetters: boolean = false
) => {
  if (notEmpty && txt.trim().length < 1) {
    return "Required Field";
  }

  if (twoCharsMin && txt.trim().length < 2) {
    return "Field is too short";
  }

  if (startWithLetters && !/^.*[\p{L}\s]{2,}.*$/u.test(txt.trim())) {
    return "Must start with at least two letter.";
  }

  if (onlyLetters && !/^[\p{L}\s'`-]*$/u.test(txt.trim())) {
    return "Please use letters only";
  }

  return "";
};

export const emailValidte = (email: string) => {
  if (email.trim().length < 1) {
    return "Email is required";
  }

  if (!isEmailValid(email)) {
    return "Must be a valid email";
  }

  return "";
};

export const isEmailValid = (email: string) => {
  const isMatch = email.match(
    /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
  );

  return isMatch;
};

export const isPhoneValid = (phone: string) => {
  const isMatch = phone.match(/^[0-9 ().+-]+$/);

  return isMatch && phone.length > 8;
};

export const passwordValidate = (p: string) => {
  if (p.length === 0) {
    return "Password is required";
  }
  if (p.length < 8) {
    return "Password must be at least 8 characters";
  }
  if (p.length > 20) {
    return "Password must be less than 20 characters";
  }
  if (p.search(/[a-z]/i) < 0) {
    return "Password should contain at least one letter.";
  }
  if (p.search(/[0-9]/) < 0) {
    return "Password must contain at least one digit.";
  }

  if (!/^[A-Za-z0-9!#$%^&*()_+-=]*$/u.test(p)) {
    return "Some characters are not allowed.";
  }

  return "";
};
