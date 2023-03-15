import { useState } from "react";

export function useFormErrors<T>(
  errModel: T
): [(field: string, errTxt: string) => boolean, (field: string) => void, T] {
  const [errors, setErrors] = useState(errModel);

  const updateFieldError = (field: string, errTxt: string) => {
    const isValid = errTxt === "" ? true : false;

    setErrors((currentProps) => ({
      ...currentProps,
      [field]: errTxt,
    }));

    return isValid;
  };

  const clearError = (field: string) => {
    setErrors((currentProps) => ({
      ...currentProps,
      [field]: "",
    }));
  };

  return [updateFieldError, clearError, errors];
}
