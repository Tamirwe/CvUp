import { Autocomplete, TextField } from "@mui/material";
import { useEffect, useState } from "react";
import { IMailsList } from "../../models/GeneralModels";
import { isEmailValid } from "../../utils/Validation";

interface IProps {
  listEmailsTo: IMailsList[];
  listDefaultEmails: IMailsList[];
}

export const EmailsToControl = (props: IProps) => {
  const [emailsToList, setEmailsToList] = useState<IMailsList[]>([]);
  const [inputVal, setInputVal] = useState("");
  const [vals, setVals] = useState<IMailsList[]>([]);

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

  const filterEmailToList = (emailsPicked: IMailsList[]) => {
    const options = props.listEmailsTo.filter((x) =>
      emailsPicked.every((y) => y.email !== x.email)
    );

    setEmailsToList(options);
  };

  return (
    <Autocomplete
      sx={{ paddingTop: 1, paddingBottom: 3 }}
      multiple
      id="tags-standard"
      options={emailsToList}
      getOptionLabel={(option) => {
        return option ? (option.name ? option.name : option.email) : "";
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
              emailsToList.findIndex((x) => x.email === newInputValue) === -1
            ) {
              setEmailsToList((emailsToList) => [
                ...emailsToList,
                { email: newInputValue, name: "", userTyped: true },
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
