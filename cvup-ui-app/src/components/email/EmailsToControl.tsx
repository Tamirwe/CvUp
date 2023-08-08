import { Autocomplete, TextField } from "@mui/material";
import { useEffect, useState } from "react";
import { IEmailsAddress } from "../../models/GeneralModels";
import { isEmailValid } from "../../utils/Validation";

interface IProps {
  optionsList: IEmailsAddress[];
  listValues: IEmailsAddress[];
  onChange: (vals: IEmailsAddress[]) => void;
}

export const EmailsToControl = (props: IProps) => {
  const [emailsToList, setEmailsToList] = useState<IEmailsAddress[]>([]);
  const [inputVal, setInputVal] = useState("");
  const [vals, setVals] = useState<IEmailsAddress[]>([]);

  useEffect(() => {
    setEmailsToList([...props.optionsList]);
  }, [props.optionsList]);

  useEffect(() => {
    if (props.listValues.length) {
      setVals([...props.listValues]);

      if (props.optionsList.length) {
        filterEmailToList(props.listValues);
      }
    }
  }, [props.listValues, props.optionsList]);

  const filterEmailToList = (emailsPicked: IEmailsAddress[]) => {
    const options = props.optionsList.filter((x) =>
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
          props.onChange(newValue);
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
