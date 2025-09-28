import { useEffect, useRef, useState } from "react";
import styles from "./OHLCinput.module.scss";

interface IProps {
  // onSaved: () => void;
  // onCancel: () => void;
}

export const OHLCinput = () => {
  const openTxtRef = useRef(null);
  const highTxtRef = useRef(null);
  const lowTxtRef = useRef(null);
  const closeTxtRef = useRef(null);

  const [openTxt, setOpenTxt] = useState("");
  const [highTxt, setHighTxt] = useState("");
  const [lowTxt, setLowTxt] = useState("");
  const [closeTxt, setCloseTxt] = useState("");

  let lastKeyPressed = "";

  const handleKeyDown = (
    e: React.KeyboardEvent<HTMLInputElement>,
    nextTarget?: HTMLInputElement,
    prevTarget?: HTMLInputElement
  ) => {
    lastKeyPressed = e.key;

    console.log("Key pressed:", lastKeyPressed);

    const target = e.target as HTMLInputElement;

    if (
      nextTarget &&
      target.selectionStart! > 2 &&
      lastKeyPressed !== "ArrowLeft" &&
      lastKeyPressed !== "Backspace" &&
      lastKeyPressed !== "Delete"
    ) {
      setTimeout(() => {
        console.log(112);
        nextTarget && nextTarget.focus();
        nextTarget!.setSelectionRange(0, 0);
      }, 50);
    }

    if (
      prevTarget &&
      lastKeyPressed === "ArrowLeft" &&
      target.selectionStart! < 1
    ) {
      setTimeout(() => {
        console.log(111);
        prevTarget && prevTarget.focus();
        prevTarget.setSelectionRange(3, 3);
      }, 50);
    }

    if (
      prevTarget &&
      lastKeyPressed === "Backspace" &&
      target.selectionStart! < 1
    ) {
      setTimeout(() => {
        console.log(111);
        prevTarget && prevTarget.focus();
        prevTarget.setSelectionRange(4, 4);
      }, 50);
    }
  };

  useEffect(() => {}, []);

  const validateInputKey = () => {
    return (
      (lastKeyPressed >= "0" && lastKeyPressed <= "9") ||
      lastKeyPressed === "ArrowLeft" ||
      lastKeyPressed === "Backspace" ||
      lastKeyPressed === "Delete"
    );
  };

  return (
    <div className={styles.wrapper}>
      <div className={styles.inputWrapper}>
        <div>OPEN</div>
        <input
          id="openTxt"
          type="text"
          maxLength={4}
          className={styles.txtInput}
          ref={openTxtRef}
          onKeyDown={(event) =>
            handleKeyDown(
              event,
              highTxtRef.current as unknown as HTMLInputElement,
              undefined
            )
          }
          onChange={(e) => {
            if (validateInputKey()) {
              setOpenTxt(e.target.value);
            }
          }}
          value={openTxt}
        />
      </div>
      <div className={styles.inputWrapper}>
        <div>HIGH</div>
        <input
          id="highTxt"
          type="text"
          maxLength={4}
          className={styles.txtInput}
          ref={highTxtRef}
          onKeyDown={(event) =>
            handleKeyDown(
              event,
              lowTxtRef.current as unknown as HTMLInputElement,
              openTxtRef.current as unknown as HTMLInputElement
            )
          }
          onChange={(e) => {
            if (validateInputKey()) {
              setHighTxt(e.target.value);
            }
          }}
          value={highTxt}
        />
      </div>
      <div className={styles.inputWrapper}>
        <div>LOW</div>
        <input
          id="lowTxt"
          type="text"
          maxLength={4}
          className={styles.txtInput}
          ref={lowTxtRef}
          onKeyDown={(event) =>
            handleKeyDown(
              event,
              closeTxtRef.current as unknown as HTMLInputElement,
              highTxtRef.current as unknown as HTMLInputElement
            )
          }
          onChange={(e) => {
            if (validateInputKey()) {
              setLowTxt(e.target.value);
            }
          }}
          value={lowTxt}
        />
      </div>
      <div className={styles.inputWrapper}>
        <div>CLOSE</div>
        <input
          id="closeTxt"
          type="text"
          maxLength={4}
          className={styles.txtInput}
          ref={closeTxtRef}
          onKeyDown={(event) =>
            handleKeyDown(
              event,
              undefined,
              lowTxtRef.current as unknown as HTMLInputElement
            )
          }
          onChange={(e) => {
            if (validateInputKey()) {
              setCloseTxt(e.target.value);
            }
          }}
          value={closeTxt}
        />
      </div>
    </div>
  );
};
