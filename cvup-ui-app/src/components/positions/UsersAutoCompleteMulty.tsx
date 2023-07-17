import { Autocomplete, TextField } from "@mui/material";
import { useEffect, useState } from "react";
import { IUser } from "../../models/AuthModels";

interface IProps {
  options: IUser[];
  valueIds: number[];
  onChange: (value: number[]) => void;
}

export const UsersAutoCompleteMulty = (props: IProps) => {
  const [optionsList, setOptionsList] = useState<IUser[]>([]);
  const [valuesList, setValuesList] = useState<IUser[]>([]);

  useEffect(() => {
    setOptionsList([...props.options]);
  }, [props.options]);

  useEffect(() => {
    if (props.options.length) {
      setValuesList([
        ...props.options.filter((x) => props.valueIds.indexOf(x.id) > -1),
      ]);
    }
  }, [props.valueIds, props.options]);

  return (
    <Autocomplete
      multiple
      id="tags-standard"
      options={optionsList}
      getOptionLabel={(option) => {
        return `${option.firstName} ${option.lastName} ${option.id}`;
      }}
      sx={{ "& .MuiAutocomplete-inputRoot": { padding: "8px" } }}
      value={valuesList}
      onChange={(event, newValue) => {
        if (event && event.type === "click") {
          props.onChange(newValue.map((item) => item.id));
        }
      }}
      renderInput={(params) => (
        <TextField
          {...params}
          variant="outlined"
          label="Interviewers"
          placeholder="Interviewers"
        />
      )}
    />
  );
};
