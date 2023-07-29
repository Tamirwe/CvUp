import {
  Button,
  Checkbox,
  FormControl,
  InputBase,
  Link,
  MenuItem,
  Select,
  Stack,
  ToggleButton,
  ToggleButtonGroup,
} from "@mui/material";
import { styled } from "@mui/material/styles";
import { useEffect, useState } from "react";
import {
  MdAdjust,
  MdFilterTiltShift,
  MdKeyboardArrowDown,
  MdKeyboardArrowUp,
  MdOutlineClose,
  MdOutlineSearch,
} from "react-icons/md";
import useDebounce from "../../Hooks/useDebounce";
import { ISearchModel } from "../../models/GeneralModels";

const Search = styled("div")(({ theme }) => ({
  position: "relative",
  borderRadius: "19px",
  backgroundColor: "#e7e7e742",
  display: "flex",
  padding: " 8px 0",
  "&:hover": {
    backgroundColor: "#dddddd69",
  },
  marginLeft: 0,
  width: "100%",
  [theme.breakpoints.up("sm")]: {
    marginLeft: theme.spacing(1),
  },
}));

const SearchIconWrapper = styled("div")(({ theme }) => ({
  cursor: "pointer",
  padding: theme.spacing(0, 2),
  height: "100%",
  // position: "absolute",
  // pointerEvents: "none",
  display: "flex",
  alignItems: "center",
  justifyContent: "center",
  color: "#0000008a",
  fontSize: "1.5rem",
  zIndex: 999,
}));

const IconWrapper = styled("div")(({ theme }) => ({
  cursor: "pointer",
  padding: theme.spacing(0, 2),
  right: 0,
  height: "100%",
  // position: "absolute",
  // pointerEvents: "none",
  display: "flex",
  alignItems: "center",
  justifyContent: "center",
  color: "#0000008a",
  fontSize: "1.5rem",
  zIndex: 999,
}));

const StyledInputBase = styled(InputBase)(({ theme }) => ({
  color: "inherit",
  width: "100%",

  "& .MuiInputBase-input": {
    padding: 0,
    margin: "0",
    direction: "rtl",
    // vertical padding + font size from searchIcon
    // paddingLeft: `calc(1em + ${theme.spacing(4)})`,
    // paddingRight: `calc(1em + ${theme.spacing(4)})`,
    transition: theme.transitions.create("width"),
    width: "100%",
    [theme.breakpoints.up("sm")]: {
      "&:focus": {
        // width: "20ch",
      },
    },
  },
}));

interface IProps {
  advancedSearch?: boolean;
  exactButtons?: boolean;
  onSearch: (value: string) => void;
}

export const SearchControl = ({
  onSearch,
  advancedSearch = false,
  exactButtons = false,
}: IProps) => {
  const [showAdvancedSearch, setShowAdvancedSearch] = useState(false);

  const [value, setValue] = useState("");
  const [searchVals, setSearchVals] = useState<ISearchModel>({
    value: "",
    exact: true,
    advancedValue: "",
    advancedExact: true,
  });
  const debouncedValue = useDebounce<ISearchModel>(searchVals, 700);

  useEffect(() => {
    onSearch(value);
  }, [debouncedValue]);

  const handleValueChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setSearchVals((currentProps) => ({
      ...currentProps,
      value: event.target.value,
    }));
    //setValue(event.target.value);
  };

  const handleAdvancedValueChange = (
    event: React.ChangeEvent<HTMLInputElement>
  ) => {
    setSearchVals((currentProps) => ({
      ...currentProps,
      advancedValue: event.target.value,
    }));
  };

  const handleKeyDown = (event: React.KeyboardEvent<HTMLInputElement>) => {
    if (event.key === "Enter") {
      search();
    }
  };

  const search = () => {
    onSearch(value);
  };

  return (
    <Stack>
      <Stack direction="row" alignItems={"center"}>
        <Search sx={{ direction: "rtl" }}>
          <SearchIconWrapper>
            <MdOutlineSearch onClick={search} />
          </SearchIconWrapper>
          <StyledInputBase
            placeholder="Search…"
            value={searchVals.value}
            onChange={handleValueChange}
            onKeyDown={handleKeyDown}
          />

          {value && (
            <IconWrapper sx={{ padding: "0 4px" }}>
              <MdOutlineClose onClick={() => setValue("")} />
            </IconWrapper>
          )}
          {advancedSearch && (
            <IconWrapper sx={{ padding: "0 4px" }}>
              {showAdvancedSearch ? (
                <MdKeyboardArrowUp
                  onClick={() => setShowAdvancedSearch(false)}
                />
              ) : (
                <MdKeyboardArrowDown
                  onClick={() => setShowAdvancedSearch(true)}
                />
              )}
            </IconWrapper>
          )}
        </Search>

        {exactButtons && (
          <ToggleButtonGroup
            sx={{
              direction: "ltr",
              "& .MuiButtonBase-root": { padding: "3px", fontSize: "0.7rem" },
            }}
            color="primary"
            value={searchVals.exact ? "exact" : "approximate"}
            exclusive
            size="small"
            onChange={(event) => {
              event.stopPropagation();
              event.preventDefault();
              setSearchVals((currentProps) => ({
                ...currentProps,
                exact: !searchVals.exact,
              }));
            }}
            aria-label="Platform"
          >
            <ToggleButton value="approximate" title="Approximate Search">
              AP
            </ToggleButton>
            <ToggleButton value="exact" title="Exact Search">
              Ex
            </ToggleButton>
          </ToggleButtonGroup>
        )}
      </Stack>
      {showAdvancedSearch && (
        <Stack direction="row" pt={1} alignItems={"center"}>
          <Search sx={{ direction: "rtl" }}>
            <SearchIconWrapper>
              <MdOutlineSearch onClick={search} />
            </SearchIconWrapper>
            <StyledInputBase
              placeholder="Search in results…"
              value={value}
              onChange={handleAdvancedValueChange}
              onKeyDown={handleKeyDown}
            />

            {value && (
              <IconWrapper sx={{ padding: "0 4px" }}>
                <MdOutlineClose onClick={() => setValue("")} />
              </IconWrapper>
            )}
          </Search>
          {exactButtons && (
            <ToggleButtonGroup
              sx={{
                direction: "ltr",
                "& .MuiButtonBase-root": { padding: "3px", fontSize: "0.7rem" },
              }}
              color="primary"
              value={searchVals.advancedExact ? "exact" : "approximate"}
              exclusive
              size="small"
              onChange={(event) => {
                event.stopPropagation();
                event.preventDefault();
                setSearchVals((currentProps) => ({
                  ...currentProps,
                  advancedExact: !searchVals.advancedExact,
                }));
              }}
              aria-label="Platform"
            >
              <ToggleButton value="approximate" title="Approximate Search">
                AP
              </ToggleButton>
              <ToggleButton value="exact" title="Exact Search">
                Ex
              </ToggleButton>
            </ToggleButtonGroup>
          )}
        </Stack>
      )}
    </Stack>
  );
};
