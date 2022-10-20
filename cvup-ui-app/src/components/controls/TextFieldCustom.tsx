import { IconButton, InputAdornment, TextField } from "@mui/material";
import { MdOutlineVisibility } from "react-icons/md";
import { IconType } from "react-icons";
import React from "react";

interface TextFieldCustomInterface {
  value?: string;
  id?: string;
  label?: string;
  variant?: "outlined" | "standard" | "filled" | undefined;
  margin?: "none" | "dense" | "normal" | undefined;
  helperText?: string;
  placeholder?: string;
  iconButton?: IconType;
  error?: boolean;
  fullWidth?: boolean;
  disabled?: boolean;
  required?: boolean;
  multiline?: boolean;
  rows?: number;
  onIconClick?: () => void;
  onChange?: (event: object) => void;
  type?: string;
}

export const TextFieldCustom = ({
  value = "",
  id = "",
  label = "",
  variant = "outlined",
  helperText = "",
  placeholder = "",
  iconButton = undefined,
  error = false,
  fullWidth = true,
  disabled = false,
  required = false,
  multiline = false,
  rows = 1,
  type = "text",
  margin = "normal",
  onIconClick = undefined,
  onChange = undefined,
}: TextFieldCustomInterface) => {
  return (
    <TextField
      required={required}
      fullWidth={fullWidth}
      disabled={disabled}
      error={error}
      multiline={multiline}
      rows={rows}
      id={id}
      label={label}
      variant={variant}
      value={value}
      helperText={helperText}
      placeholder={placeholder}
      type={type}
      margin={margin}
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
