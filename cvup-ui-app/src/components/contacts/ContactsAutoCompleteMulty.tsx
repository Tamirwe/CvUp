import { Autocomplete, TextField } from "@mui/material";
import { useEffect, useState } from "react";
import { IContact, IMailsList } from "../../models/GeneralModels";
import { isEmailValid } from "../../utils/Validation";

interface IProps {
  allRecordslist: IContact[];
  selectedRecords: IContact[];
}

export const ContactsAutoCompleteMulty = (props: IProps) => {
  const [autoCompList, setAutoCompList] = useState<IContact[]>([]);
  const [inputVal, setInputVal] = useState("");
  const [vals, setVals] = useState<IContact[]>([]);

  useEffect(() => {
    setAutoCompList([...props.allRecordslist]);
  }, [props.allRecordslist]);

  useEffect(() => {
    if (props.selectedRecords.length) {
      setVals([...props.selectedRecords]);

      if (props.allRecordslist.length) {
        filterSelectedFromList(props.selectedRecords);
      }
    }
  }, [props.selectedRecords, props.allRecordslist]);

  const filterSelectedFromList = (selectedRecords: IContact[]) => {
    const options = props.allRecordslist.filter((x) =>
      selectedRecords.every((y) => y.id !== x.id)
    );

    setAutoCompList(options);
  };

  return (
    <Autocomplete
      multiple
      id="tags-standard"
      options={autoCompList}
      getOptionLabel={(option) => {
        return `${option.firstName} ${option.lastName} - ${option.customerName}`;
      }}
      value={vals}
      onChange={(event, newValue) => {
        if (event && event.type === "click") {
          setVals(newValue);
          filterSelectedFromList(newValue);
        }
      }}
      // onClose={() => {
      //   filterEmailToList(vals);
      // }}
      inputValue={inputVal}
      onInputChange={(event, newInputValue) => {
        if (event && event.type !== "blur") {
          setInputVal(newInputValue);
          // if (isEmailValid(newInputValue)) {
          //   if (
          //     autoCompList.findIndex((x) => x.customerName === newInputValue) === -1
          //   ) {
          //     setAutoCompList((autoCompList) => [
          //       ...autoCompList,
          //       { email: newInputValue, name: "", userTyped: true },
          //     ]);
          //   }
          // }
        }
      }}
      renderInput={(params) => (
        <TextField
          {...params}
          variant="outlined"
          label="Contact person"
          placeholder="Contact person"
        />
      )}
    />
  );
};
