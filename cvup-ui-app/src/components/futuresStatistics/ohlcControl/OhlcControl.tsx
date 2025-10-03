import {
  forwardRef,
  MutableRefObject,
  useEffect,
  useImperativeHandle,
  useRef,
  useState,
} from "react";
import { InputMask, MaskOptions } from "@react-input/mask";
import styles from "./OhlcControl.module.scss";
import { Iohlc } from "../../../models/FuStatModel";

//  const ChildComponent = forwardRef<ChildComponentRef, ChildComponentProps>(
//       ({ initialValue }, ref) => {

interface IProps {
  editData?: Iohlc | null;
  onChange: (ohlc: Iohlc | null) => void;
  onSave: () => void;
}

export interface OhlcControlRefType {
  resetData: () => void;
}

export const OhlcControl = forwardRef<OhlcControlRefType, IProps>(
  ({ editData, onChange, onSave }: IProps, ref) => {
    useImperativeHandle(ref, () => ({
      resetData,
    }));

    const [defaultData, setDefaultData] = useState<Iohlc | null>(null);
    const [ohlcId, setOhlcId] = useState(0);
    const [dateTxt, setDateTxt] = useState("");
    const [dateErrorTxt, setDateErrorTxt] = useState(false);

    const [ohlcData, setOhlcData] = useState("");
    const [ohlcDataError, setOhlcDataError] = useState(false);

    const updateDefaultData = () => {
      let ohlcDayDate: Date;

      if (defaultData && defaultData.id && defaultData.statisticDate) {
        ohlcDayDate = new Date(defaultData.statisticDate);
        setOhlcId(defaultData.id);
        setOhlcData(
          `${defaultData.open}-${defaultData.high}-${defaultData.low}-${defaultData.close}`
        );
      } else {
        ohlcDayDate = new Date();
      }

      const dateMaskFormat = `${
        (ohlcDayDate.getMonth() < 9 ? "0" : "") + (ohlcDayDate.getMonth() + 1)
      }/${
        (ohlcDayDate.getDate() < 10 ? "0" : "") + ohlcDayDate.getDate()
      }/${ohlcDayDate.getFullYear()}`;

      setDateTxt(dateMaskFormat);
    };

    const resetData = () => {
      setOhlcId(0);
      setDefaultData(null);
      updateDefaultData();
      setOhlcData("");
      setDateErrorTxt(false);
      setOhlcDataError(false);
    };

    useEffect(() => {
      setDefaultData(editData || null);
    }, [editData]);

    useEffect(() => {
      updateDefaultData();
    }, [defaultData]);

    const handleDateOnblur = (dateTxt: string) => {
      const [isValid, date] = validatDate();

      if (isValid) {
        const dateFormat = `${
          date.getMonth() < 9
            ? "0" + (date.getMonth() + 1)
            : date.getMonth() + 1
        }/${date.getDate() < 10 ? "0" + date.getDate() : date.getDate() + ""}/${
          date.getFullYear() + ""
        }`;

        setDateTxt(dateFormat);
      }

      // handleOnChange(isValid, !ohlcDataError);
      setDateErrorTxt(!isValid);
    };

    const validatDate = (ohlcVal: string = ohlcData): [boolean, Date] => {
      let isValid = true;

      const dateArr = dateTxt.split("/");
      const date = new Date(
        `${dateArr[2].trim()}-${dateArr[0].trim()}-${dateArr[1].trim()}`
      );

      if (isNaN(date.getDate())) {
        isValid = false;
      }

      return [isValid, date];
    };

    const handleOhlcOnblur = () => {
      const isValid = validateOhlc();

      // handleOnChange(!dateErrorTxt, isValid);
      setOhlcDataError(!isValid);
    };

    const validateOhlc = (ohlcVal: string = ohlcData) => {
      let isValid = true;
      const ohlcDataArr = ohlcVal.split("-");

      for (let i = 0; i < ohlcDataArr.length; i++) {
        const element = ohlcDataArr[i];

        if (element.length !== 4) {
          isValid = false;
          break;
        }
      }

      return isValid;
    };

    const handleOnChange = (
      isDataValid: boolean,
      isOhlcValid: boolean,
      date: Date | null,
      ohlc: string = ohlcData
    ) => {
      if (isDataValid && isOhlcValid && ohlc) {
        let ohlcDate = null;

        if (date) {
          ohlcDate = date;
        } else {
          const dateArr = dateTxt.split("/");
          ohlcDate = new Date(
            `${dateArr[2].trim()}-${dateArr[0].trim()}-${dateArr[1].trim()}`
          );
        }

        const ohlcDataArr = ohlc.split("-");

        const newOhlc: Iohlc = {
          id: ohlcId || 0,
          statisticDate: ohlcDate,
          open: parseInt(ohlcDataArr[0]),
          high: parseInt(ohlcDataArr[1]),
          low: parseInt(ohlcDataArr[2]),
          close: parseInt(ohlcDataArr[3]),
        };

        onChange(newOhlc);
      } else {
        onChange(null);
      }
    };

    const handleOhlcKeyUp = (e: React.KeyboardEvent<HTMLInputElement>) => {
      const lastKeyPressed = e.key;

      if (lastKeyPressed === "Enter") {
        handleOhlcOnblur();

        setTimeout(() => {
          onSave();
        }, 150);
      }

      // console.log("Key pressed:", lastKeyPressed);
    };

    return (
      <div className={styles.wrapper}>
        <div className={styles.inputWrapper}>
          <label className={styles.title}>Date (mm/dd/yyyy)</label>
          <InputMask
            className={styles.dateInput}
            mask="  /  /    "
            replacement={{ " ": /\d/ }}
            placeholder="mm/dd/yyyy"
            showMask
            separate
            onChange={(e: React.ChangeEvent<HTMLInputElement>) => {
              setDateTxt(e.target.value);
              setOhlcId(0);
              const [isValid, date] = validatDate();
              handleOnChange(isValid, !ohlcDataError, date, ohlcData);
            }}
            onBlur={(e: React.FocusEvent<HTMLInputElement, Element>) =>
              handleDateOnblur(e.target.value)
            }
            value={dateTxt}
          />
          <label className={styles.error}>
            {dateErrorTxt && "*Invalid Date"}
          </label>
        </div>
        <div className={styles.inputWrapper}>
          <label className={styles.title}>OHLC</label>
          <InputMask
            className={styles.ohlcInput}
            mask="____-____-____-____"
            replacement={{ _: /\d/ }}
            placeholder="OPEN-HIGH-LOW-CLOSE"
            onChange={(e: React.ChangeEvent<HTMLInputElement>) => {
              setOhlcData(e.target.value);

              const isValid = validateOhlc(e.target.value);

              handleOnChange(!dateErrorTxt, isValid, null, e.target.value);
            }}
            onBlur={handleOhlcOnblur}
            onKeyUp={handleOhlcKeyUp}
            value={ohlcData}
          />
          <label className={styles.error}>
            {" "}
            {ohlcDataError && "*Invalid (XXXX-XXXX-XXXX-XXXX)"}
          </label>
        </div>
      </div>
    );
  }
);
