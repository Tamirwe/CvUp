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
  MdRule,
  MdSort,
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
  shoeAdvancedIcon?: boolean;
  onSearch: (value: ISearchModel) => void;
  onShowAdvanced?: (isShow: boolean) => void;
  records?: number;
  onSort?: (sortBy: string, dir: string) => void;
}

export const SearchControl = ({
  onSearch,
  onShowAdvanced,
  shoeAdvancedIcon = false,
  records,
  onSort,
}: IProps) => {
  const [showAdvancedSearch, setShowAdvancedSearch] = useState(false);

  //const [value, setValue] = useState("");
  const [searchVals, setSearchVals] = useState<ISearchModel>({
    value: "",
    exact: false,
    advancedValue: "",
  });
  const [sortBy, setSortBy] = useState("scoreSort");
  const [sortByScoreDesc, setSortByScoreDesc] = useState(true);
  const [sortByCvDateDesc, setSortByCvDateDesc] = useState(false);

  const debouncedValue = useDebounce<ISearchModel>(searchVals, 1000);

  useEffect(() => {
    onSearch(searchVals);
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
    onSearch(searchVals);
  };

  const handleSort = (cvDateSortClick: boolean, ScoreSortClick: boolean) => {
    let sortDir = "";

    if (cvDateSortClick) {
      if (sortByCvDateDesc) {
        sortDir = "asc";
      } else {
        sortDir = "desc";
      }
      onSort && onSort("cvDateSort", sortDir);
      setSortByScoreDesc(false);
      setSortBy("cvDateSort");
    } else {
      if (sortByScoreDesc) {
        sortDir = "asc";
      } else {
        sortDir = "desc";
      }
      onSort && onSort("scoreSort", sortDir);
      setSortByCvDateDesc(false);
      setSortBy("scoreSort");
    }
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

          {searchVals.value && (
            <IconWrapper sx={{ padding: "0 4px" }}>
              <MdOutlineClose
                onClick={() => {
                  setSearchVals((currentProps) => ({
                    ...currentProps,
                    value: "",
                  }));
                  setSearchVals((currentProps) => ({
                    ...currentProps,
                    advancedValue: "",
                  }));
                }}
              />
            </IconWrapper>
          )}
          {shoeAdvancedIcon && (
            <IconWrapper sx={{ padding: "0 4px" }}>
              {showAdvancedSearch ? (
                <MdKeyboardArrowUp
                  onClick={() => {
                    setShowAdvancedSearch(false);
                    onShowAdvanced && onShowAdvanced(false);
                    setSearchVals((currentProps) => ({
                      ...currentProps,
                      advancedValue: "",
                    }));
                    setSearchVals((currentProps) => ({
                      ...currentProps,
                      exact: false,
                    }));
                  }}
                />
              ) : (
                <MdKeyboardArrowDown
                  onClick={() => {
                    setShowAdvancedSearch(true);
                    onShowAdvanced && onShowAdvanced(false);
                  }}
                />
              )}
            </IconWrapper>
          )}
        </Search>

        {showAdvancedSearch && (
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
              value={searchVals.advancedValue}
              onChange={handleAdvancedValueChange}
              onKeyDown={handleKeyDown}
            />

            {searchVals.advancedValue && (
              <IconWrapper sx={{ padding: "0 4px" }}>
                <MdOutlineClose
                  onClick={() => {
                    setSearchVals((currentProps) => ({
                      ...currentProps,
                      advancedValue: "",
                    }));
                  }}
                />
              </IconWrapper>
            )}
          </Search>
          {searchVals.value && (
            <div
              style={{
                whiteSpace: "nowrap",
                direction: "ltr",
                padding: "0 5px",
              }}
            >
              {records === 300 ? `300 ...` : `${records} rec`}
            </div>
          )}
          <ToggleButtonGroup
            sx={{
              direction: "ltr",
              "& .MuiButtonBase-root": {
                padding: "5px 3px",
                fontSize: "1.0rem",
              },
            }}
            color={sortByCvDateDesc ? "primary" : "secondary"}
            value={sortBy}
            exclusive
            size="small"
            onChange={(event) => {
              event.stopPropagation();
              event.preventDefault();
              setSortByCvDateDesc(!sortByCvDateDesc);
              handleSort(true, false);
            }}
            aria-label="Platform"
          >
            <ToggleButton value="cvDateSort" title="Sort by cv sent date">
              <MdSort />
            </ToggleButton>
          </ToggleButtonGroup>
          <ToggleButtonGroup
            sx={{
              direction: "ltr",
              "& .MuiButtonBase-root": {
                padding: "5px 3px",
                fontSize: "1.0rem",
              },
            }}
            color={sortByScoreDesc ? "primary" : "secondary"}
            value={sortBy}
            exclusive
            size="small"
            onChange={(event) => {
              event.stopPropagation();
              event.preventDefault();
              setSortByScoreDesc(!sortByScoreDesc);
              handleSort(false, true);
            }}
            aria-label="Platform"
          >
            <ToggleButton value="scoreSort" title="Sort by score">
              <MdRule />
            </ToggleButton>
          </ToggleButtonGroup>
        </Stack>
      )}
    </Stack>
  );
};
