import { useState } from "react";
import { InputMask } from "@react-input/mask";
import styles from "./OhlcControl.module.scss";

interface IProps {
  statDate?: Date;
  // onSaved: () => void;
  // onCancel: () => void;
}

export const OhlcControl = ({ statDate = new Date() }: IProps) => {
  const [dateTxt, setDateTxt] = useState(
    `${
      statDate.getMonth() < 9
        ? "0" + (statDate.getMonth() + 1)
        : statDate.getMonth() + 1
    }/${statDate.getDate()}/${statDate.getFullYear()}`
  );
  const [dateErrorTxt, setDateErrorTxt] = useState(false);

  const [ohlcData, setOhlcData] = useState("");
  const [ohlcDataError, setOhlcDataError] = useState(false);

  // const [ohlcDate, setOhlcDate] = useState(date.toLocaleDateString("en-US"));

  const validateDate = (dateTxt: string) => {
    let isValid = true;

    const dateArr = dateTxt.split("/");
    const date = new Date(
      `${dateArr[2].trim()}-${dateArr[0].trim()}-${dateArr[1].trim()}`
    );

    if (isNaN(date.getDate())) {
      isValid = false;
    } else {
      const dateTxt = `${
        date.getMonth() < 9 ? "0" + (date.getMonth() + 1) : date.getMonth() + 1
      }/${date.getDate() < 9 ? "0" + date.getDate() : date.getDate() + ""}/${
        date.getFullYear() + ""
      }`;

      setDateTxt(dateTxt);
    }

    setDateErrorTxt(!isValid);
  };

  const validateOhlcData = (ohlcData: string) => {
    let isValid = true;
    const ohlcDataArr = ohlcData.split("-");

    for (let i = 0; i < ohlcDataArr.length; i++) {
      const element = ohlcDataArr[i];

      if (element.length !== 4) {
        isValid = false;
        break;
      }
    }

    setOhlcDataError(!isValid);
  };

  return (
    <div className={styles.wrapper}>
      <div className={styles.inputWrapper}>
        <label>Date (mm/dd/yyyy)</label>
        <InputMask
          className={styles.dateInput}
          mask="  /  /    "
          replacement={{ " ": /\d/ }}
          placeholder="mm/dd/yyyy"
          showMask
          separate
          onChange={(e: React.ChangeEvent<HTMLInputElement>) => {
            setDateTxt(e.target.value);
          }}
          onBlur={(e: React.FocusEvent<HTMLInputElement, Element>) =>
            validateDate(e.target.value)
          }
          value={dateTxt}
        />
        <label className={styles.error}>
          {dateErrorTxt && "*Invalid Date"}
        </label>
      </div>
      <div className={styles.inputWrapper}>
        <label>OHLC</label>
        <InputMask
          className={styles.ohlcInput}
          mask="____-____-____-____"
          replacement={{ _: /\d/ }}
          placeholder="OPEN-HIGH-LOW-CLOSE"
          onChange={(e: React.ChangeEvent<HTMLInputElement>) => {
            setOhlcData(e.target.value);
          }}
          onBlur={(e: React.FocusEvent<HTMLInputElement, Element>) =>
            validateOhlcData(e.target.value)
          }
          value={ohlcData}
        />
        <label className={styles.error}>
          {" "}
          {ohlcDataError && "*Invalid (XXXX-XXXX-XXXX-XXXX)"}
        </label>
      </div>
    </div>
  );
};
