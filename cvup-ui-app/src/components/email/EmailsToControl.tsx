import { Autocomplete, TextField } from "@mui/material";
import { useEffect, useState } from "react";
import { IEmailsAddress } from "../../models/GeneralModels";
import { isEmailValid } from "../../utils/Validation";

interface IProps {
  listEmailsTo: IEmailsAddress[];
  listDefaultEmails: IEmailsAddress[];
}

export const EmailsToControl = (props: IProps) => {
  const [emailsToList, setEmailsToList] = useState<IEmailsAddress[]>([]);
  const [inputVal, setInputVal] = useState("");
  const [vals, setVals] = useState<IEmailsAddress[]>([]);

  useEffect(() => {
    setEmailsToList([...props.listEmailsTo]);
  }, [props.listEmailsTo]);

  useEffect(() => {
    if (props.listDefaultEmails.length) {
      setVals([...props.listDefaultEmails]);

      if (props.listEmailsTo.length) {
        filterEmailToList(props.listDefaultEmails);
      }
    }
  }, [props.listDefaultEmails, props.listEmailsTo]);

  const filterEmailToList = (emailsPicked: IEmailsAddress[]) => {
    const options = props.listEmailsTo.filter((x) =>
      emailsPicked.every((y) => y.Address !== x.Address)
    );

    setEmailsToList(options);
  };

  return (
    <Autocomplete
      multiple
      id="tags-standard"
      options={emailsToList}
      getOptionLabel={(option) => {
        return option ? (option.Name ? option.Name : option.Address!) : "";
      }}
      value={vals}
      onChange={(event, newValue) => {
        if (event && event.type === "click") {
          setVals(newValue);
          filterEmailToList(newValue);
        }
      }}
      // onClose={() => {
      //   filterEmailToList(vals);
      // }}
      inputValue={inputVal}
      onInputChange={(event, newInputValue) => {
        if (event && event.type !== "blur") {
          setInputVal(newInputValue);
          if (isEmailValid(newInputValue)) {
            if (
              emailsToList.findIndex((x) => x.Address === newInputValue) === -1
            ) {
              setEmailsToList((emailsToList) => [
                ...emailsToList,
                { Address: newInputValue, Name: "", userTyped: true },
              ]);
            }
          }
        }
      }}
      renderInput={(params) => (
        <TextField
          {...params}
          variant="outlined"
          label="To"
          placeholder="Email Address"
        />
      )}
    />
  );
};
