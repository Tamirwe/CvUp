import { Button, Grid } from "@mui/material";
import { useEffect, useState } from "react";
import { useFormErrors } from "../../Hooks/useFormErrors";
import { useStore } from "../../Hooks/useStore";
import {
  AlertConfirmDialogEnum,
  CrudTypesEnum,
  TextValidateTypeEnum,
} from "../../models/GeneralEnums";
import { validateTxt } from "../../utils/Validation";
import { ICustomer } from "../../models/GeneralModels";
import { Iohlc } from "../../models/FuStatModel";
import { OhlcControl } from "./ohlcControl/OhlcControl";
import styles from "./EditOHLC.module.scss";

interface IProps {
  statDate?: Date;
  // onSaved: () => void;
  // onCancel: () => void;
}

export const EditOHLC = ({ statDate = new Date() }: IProps) => {
  const { customersContactsStore, generalStore } = useStore();
  const [isDirty, setIsDirty] = useState(false);
  const [submitError, setSubmitError] = useState("");
  const [crudType, setCrudType] = useState<CrudTypesEnum>(CrudTypesEnum.Insert);
  const [formModel, setFormModel] = useState<ICustomer>({
    id: 0,
    name: "",
    descr: "",
    address: "",
  });
  const [updateFieldError, clearError, errModel] = useFormErrors({
    name: "",
  });

  useEffect(() => {
    if (customersContactsStore.selectedCustomer) {
      setCrudType(CrudTypesEnum.Update);
      setFormModel({ ...customersContactsStore.selectedCustomer });
    }
  }, []);

  const validateForm = () => {
    let isFormValid = true,
      err = "";

    err = validateTxt(formModel.name, [
      TextValidateTypeEnum.notEmpty,
      TextValidateTypeEnum.twoCharsMin,
      TextValidateTypeEnum.startWithTwoLetters,
    ]);
    isFormValid = updateFieldError("name", err) && isFormValid;

    return isFormValid;
  };

  const handleSubmit = async () => {
    setIsDirty(false);
    if (validateForm()) {
      let response;

      if (crudType === CrudTypesEnum.Insert) {
        response = await customersContactsStore.addCustomer(formModel);
      } else {
        response = await customersContactsStore.updateCustomer(formModel);
      }

      if (response.isSuccess) {
        customersContactsStore.setCustomerAddedUpdated(response.data);
        //onSaved();
      } else {
        return setSubmitError("An Error Occurred Please Try Again Later.");
      }
    }
  };

  return (
    <form noValidate spellCheck="false" className={styles.wrapper}>
      <Grid container>
        <Grid item sx={{ display: "flex" }}>
          <OhlcControl />

          <Button color="secondary" onClick={handleSubmit}>
            Save
          </Button>
        </Grid>
      </Grid>
    </form>
  );
};
