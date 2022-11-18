import { SetStateAction } from "react";
import { textFieldInterface } from "../models/AuthModels";

/* eslint-disable no-useless-escape */

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
    return "Password is required";
  }

  const isMatch = email.match(
    /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
  );

  if (!isMatch) {
    return "Must be a valid email";
  }

  return "";
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

export const validateField = (
  typeValidate: string,
  field: textFieldInterface,
  setField: (value: SetStateAction<textFieldInterface>) => void
) => {
  let isFormValid = true;
  let fieldError = "";

  switch (typeValidate) {
    case "email":
      fieldError = emailValidte(field.value);
      break;
    case "password":
      fieldError = passwordValidate(field.value);
      break;
    case "text":
      fieldError = textFieldValidte(field.value, true, true, true);
      break;
    default:
      fieldError = textFieldValidte(field.value, true, true, true);
      break;
  }

  setField((currentProps) => ({
    ...currentProps,
    error: false,
    helperText: "",
  }));

  if (fieldError) {
    isFormValid = false;
    setField((currentProps) => ({
      ...currentProps,
      error: true,
      helperText: fieldError,
    }));
  }

  return isFormValid;
};
