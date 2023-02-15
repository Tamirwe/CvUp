import { InputBase } from "@mui/material";
import { styled } from "@mui/material/styles";
import { useState } from "react";

import { CiSearch, CiCircleRemove } from "react-icons/ci";
import { useStore } from "../../Hooks/useStore";

const Search = styled("div")(({ theme }) => ({
  position: "relative",
  borderRadius: "19px",
  backgroundColor: "#e7e7e742",

  "&:hover": {
    backgroundColor: "#dddddd69",
  },
  marginLeft: 0,
  width: "100%",
  [theme.breakpoints.up("sm")]: {
    marginLeft: theme.spacing(1),
    width: "auto",
  },
}));

const SearchIconWrapper = styled("div")(({ theme }) => ({
  cursor: "pointer",
  padding: theme.spacing(0, 2),
  height: "100%",
  position: "absolute",
  // pointerEvents: "none",
  display: "flex",
  alignItems: "center",
  justifyContent: "center",
  color: "#0000008a",
  fontSize: "1.5rem",
  zIndex: 999999,
}));

const CancelIconWrapper = styled("div")(({ theme }) => ({
  cursor: "pointer",
  padding: theme.spacing(0, 2),
  right: 0,
  height: "100%",
  position: "absolute",
  // pointerEvents: "none",
  display: "flex",
  alignItems: "center",
  justifyContent: "center",
  color: "#0000008a",
  fontSize: "1.5rem",
  zIndex: 999999,
}));

const StyledInputBase = styled(InputBase)(({ theme }) => ({
  color: "inherit",
  "& .MuiInputBase-input": {
    padding: theme.spacing(1, 1, 1, 0),
    marginRight: "8px",
    direction: "rtl",
    // vertical padding + font size from searchIcon
    paddingLeft: `calc(1em + ${theme.spacing(4)})`,
    paddingRight: `calc(1em + ${theme.spacing(4)})`,
    transition: theme.transitions.create("width"),
    width: "100%",
    [theme.breakpoints.up("sm")]: {
      width: "12ch",
      "&:focus": {
        // width: "20ch",
      },
    },
  },
}));

export const SearchCands = () => {
  const { candsStore } = useStore();

  const [value, setValue] = useState("");

  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setValue(event.target.value);
  };

  const handleKeyDown = (event: React.KeyboardEvent<HTMLInputElement>) => {
    if (event.key === "Enter") {
      search();
    }
  };

  const search = () => {
    candsStore.searchCands(value);
  };

  return (
    <Search>
      <SearchIconWrapper>
        <CiSearch onClick={search} />
      </SearchIconWrapper>
      {value && (
        <CancelIconWrapper>
          <CiCircleRemove onClick={() => setValue("")} />
        </CancelIconWrapper>
      )}
      <StyledInputBase
        placeholder="Search…"
        inputProps={{ "aria-label": "search" }}
        value={value}
        onChange={handleChange}
        onKeyDown={handleKeyDown}
      />
    </Search>
  );
};
