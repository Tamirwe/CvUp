import { IconButton, InputAdornment, TextField } from "@mui/material";
import { MdOutlineVisibility } from "react-icons/md";
import { IconType } from "react-icons";
import React from "react";

interface TextFieldFormInterface {
  value?: string;
  id?: string;
  label?: string;
  variant?: "outlined" | "standard" | "filled" | undefined;
  helperText?: string;
  placeholder?: string;
  iconButton?: IconType;
  isError?: boolean;
  isFullWidth?: boolean;
  isDisabled?: boolean;
  isRequired?: boolean;
  isMultiline?: boolean;
  rowsNumber?: number;
  onIconClick?: () => void;
  onChange?: (event: object) => void;
  type?: string;
}

export const TextFieldForm = ({
  value = "",
  id = "",
  label = "",
  variant = "outlined",
  helperText = "",
  placeholder = "",
  iconButton = undefined,
  isError = false,
  isFullWidth = true,
  isDisabled = false,
  isRequired = false,
  isMultiline = false,
  rowsNumber = 2,
  type = "text",
  onIconClick = undefined,
  onChange = undefined,
}: TextFieldFormInterface) => {
  return (
    <TextField
      required={isRequired}
      fullWidth={isFullWidth}
      disabled={isDisabled}
      error={isError}
      multiline={isMultiline}
      rows={rowsNumber}
      id={id}
      label={label}
      variant={variant}
      value={value}
      helperText={helperText}
      placeholder={placeholder}
      type={type}
      onChange={onChange}
      InputProps={{
        endAdornment: iconButton ? (
          <InputAdornment position="end">
            <IconButton
              aria-label="toggle password visibility"
              onClick={onIconClick}
              edge="end"
            >
              {React.createElement(iconButton)}
            </IconButton>
          </InputAdornment>
        ) : (
          ""
        ),
      }}
    />
  );
};
