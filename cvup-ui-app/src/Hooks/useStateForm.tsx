import { Dispatch, SetStateAction, useCallback, useState } from "react";

export function useStateForm<TData, TErrors, TMessage>(
  formData: TData,
  formErrors: TErrors,
  formMessages: TMessage
): [
  TData,
  Dispatch<SetStateAction<TData>>,
  TErrors,
  TMessage,
  string,
  Dispatch<SetStateAction<string>>,
  (field: string, errTxt: string) => boolean,
  Dispatch<SetStateAction<boolean>>,
  boolean
] {
  const [frmState, setFrmState] = useState<typeof formData>(formData);
  const [frmErrs, setFrmErrs] = useState<typeof formErrors>(formErrors);
  const [frmErrsMsgs, setFrmErrsMsgs] =
    useState<typeof formMessages>(formMessages);
  const [isDirty, setIsDirty] = useState(false);
  const [frmMsgErr, setFrmMsgErr] = useState("");

  const setFieldErr = useCallback((field: string, err: string) => {
    const isValid = err === "" ? true : false;
    setFrmMsgErr("");
    setFrmErrsMsgs((currentProps) => ({
      ...currentProps,
      [field]: err,
    }));
    setFrmErrs((currentProps) => ({
      ...currentProps,
      [field]: isValid === false,
    }));

    return isValid;
  }, []);

  return [
    frmState,
    setFrmState,
    frmErrs,
    frmErrsMsgs,
    frmMsgErr,
    setFrmMsgErr,
    setFieldErr,
    setIsDirty,
    isDirty,
  ];
}
