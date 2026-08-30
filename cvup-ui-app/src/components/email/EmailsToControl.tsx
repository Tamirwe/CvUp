import { Autocomplete, TextField } from "@mui/material";
import { useState } from "react";
import { IEmailsAddress } from "../../models/GeneralModels";
import { isEmailValid } from "../../utils/Validation";

interface IProps {
  optionsList: IEmailsAddress[];
  listValues: IEmailsAddress[];
  id?: string;
  label?: string;
  placeholder?: string;
  onChange: (vals: IEmailsAddress[]) => void;
}

export const EmailsToControl = (props: IProps) => {
  const [inputVal, setInputVal] = useState("");
  const [typedEmails, setTypedEmails] = useState<IEmailsAddress[]>([]);

  const optionsList = [...props.optionsList, ...typedEmails];

  return (
    <Autocomplete
      multiple
      id={props.id || "tags-standard"}
      options={optionsList}
      filterSelectedOptions
      isOptionEqualToValue={(option, value) => option.Address === value.Address}
      getOptionLabel={(option) => {
        return option ? (option.Name ? option.Name : option.Address!) : "";
      }}
      value={props.listValues}
      onChange={(event, newValue) => {
        props.onChange(newValue);
      }}
      inputValue={inputVal}
      onInputChange={(event, newInputValue) => {
        if (event && event.type !== "blur") {
          setInputVal(newInputValue);

          if (
            isEmailValid(newInputValue) &&
            optionsList.findIndex((x) => x.Address === newInputValue) === -1
          ) {
            setTypedEmails((typedEmails) => [
              ...typedEmails,
              { Address: newInputValue, Name: "", userTyped: true },
            ]);
          }
        }
      }}
      renderInput={(params) => (
        <TextField
          {...params}
          variant="outlined"
          label={props.label || "To"}
          placeholder={props.placeholder || "Email Address"}
        />
      )}
    />
  );
};
