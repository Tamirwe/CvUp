import { Autocomplete, TextField } from "@mui/material";
import { useEffect, useState } from "react";
import { IContact } from "../../models/GeneralModels";

interface IProps {
  options: IContact[];
  valueIds: number[];
  onChange: (value: number[], customerId: number) => void;
}

export const ContactsAutoCompleteMulty = (props: IProps) => {
  const [optionsList, setOptionsList] = useState<IContact[]>([]);
  const [valuesList, setValuesList] = useState<IContact[]>([]);

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
        return `${option.firstName || ""} ${option.lastName || ""} - ${
          option.customerName
        }`;
      }}
      value={valuesList}
      sx={{ "& .MuiAutocomplete-inputRoot": { padding: "7px" } }}
      onChange={(event, newValue) => {
        if (event && event.type === "click") {
          const customerId = newValue.length > 0 ? newValue[0].customerId : 0;
          props.onChange(
            newValue.map((item) => item.id),
            customerId
          );
        }
      }}
      renderInput={(params) => (
        <TextField
          // dir="rtl"
          {...params}
          variant="outlined"
          label="Contact person"
          placeholder="Contact person"
        />
      )}
    />
  );
};
