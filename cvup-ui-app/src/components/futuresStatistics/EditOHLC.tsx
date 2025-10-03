import { Button, Grid } from "@mui/material";
import { forwardRef, useImperativeHandle, useRef, useState } from "react";
import { useStore } from "../../Hooks/useStore";
import { Iohlc } from "../../models/FuStatModel";
import { OhlcControl, OhlcControlRefType } from "./ohlcControl/OhlcControl";
import styles from "./EditOHLC.module.scss";

interface IProps {
  statDate?: Date;
}

export interface EditOHLCRefType {
  editRow: (ohlcRow: Iohlc) => void;
}

export const EditOHLC = forwardRef<EditOHLCRefType, IProps>(
  ({ statDate = new Date() }: IProps, ref) => {
    const ohlcControlRef = useRef<OhlcControlRefType>(null);
    const [ohlcControlData, setOhlcControlData] = useState<Iohlc | null>(null);

    useImperativeHandle(ref, () => ({
      editRow,
    }));

    const { futuresStatisticStore } = useStore();
    const [ohlcFormData, setOhlcFormData] = useState<Iohlc | null>(null);

    const validateForm = () => {
      if (ohlcFormData) {
        return true;
      }

      return false;
    };

    const handleSubmit = async () => {
      if (validateForm()) {
        let response;

        if (ohlcFormData?.id === 0) {
          response = await futuresStatisticStore.addDayOhlc(ohlcFormData!);
        } else {
          response = await futuresStatisticStore.updateDayOhlc(ohlcFormData!);
        }

        if (!response.isSuccess) {
          const responseErrorData = response.errorData.response.data;
          alert(
            `${responseErrorData} - ` +
              "An Error Occurred Please Try Again Later."
          );
        }
      } else {
        alert("Form not valid.");
      }
    };

    const handleOhlcChange = (ohlc: Iohlc | null) => {
      setOhlcFormData(ohlc);
    };

    const handleCancel = () => {
      ohlcControlRef.current?.resetData();
    };

    const editRow = (ohlcRow: Iohlc) => {
      setOhlcControlData(ohlcRow);
    };

    return (
      <form noValidate spellCheck="false" className={styles.wrapper}>
        <Grid container>
          <Grid item sx={{ display: "flex" }}>
            <OhlcControl
              editData={ohlcControlData}
              onChange={handleOhlcChange}
              onSave={handleSubmit}
              ref={ohlcControlRef}
            />

            <Button color="primary" onClick={handleSubmit}>
              Save
            </Button>
            <Button color="warning" onClick={handleCancel}>
              Cancel
            </Button>
          </Grid>
        </Grid>
      </form>
    );
  }
);
