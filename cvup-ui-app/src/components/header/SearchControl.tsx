import {
  InputBase,
  Stack,
  ToggleButton,
  ToggleButtonGroup,
} from "@mui/material";
import { styled } from "@mui/material/styles";
import { useEffect, useState } from "react";
import {
  MdKeyboardArrowDown,
  MdKeyboardArrowUp,
  MdOutlineClose,
  MdOutlineSearch,
  MdRefresh,
  MdRule,
  MdSort,
} from "react-icons/md";
import useDebounce from "../../Hooks/useDebounce";
import { ISearchModel } from "../../models/GeneralModels";
import { SortByEnum } from "../../models/GeneralEnums";

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
  onSort?: (sortBy: SortByEnum, dir: string) => void;
  onSortLeftLists?: (dir: string) => void;
  showSortLeft?: boolean;
  showRefreshList?: boolean;
  onRefreshLists?: () => void;
  extSearch?: ISearchModel;
}

export const SearchControl = ({
  onSearch,
  onShowAdvanced,
  shoeAdvancedIcon = false,
  records,
  onSort,
  onSortLeftLists,
  showSortLeft = false,
  showRefreshList = false,
  onRefreshLists,
  extSearch,
}: IProps) => {
  const [showAdvancedSearch, setShowAdvancedSearch] = useState(false);

  //const [value, setValue] = useState("");
  const [searchVals, setSearchVals] = useState<ISearchModel>({
    value: "",
    exact: true,
    advancedValue: "",
  });
  const [sortBy, setSortBy] = useState(SortByEnum.score);
  const [sortByScoreDesc, setSortByScoreDesc] = useState(true);
  const [sortByCvDateDesc, setSortByCvDateDesc] = useState(false);
  const [sortByLeft, setSortByLeft] = useState(true);
  const [refreshList, setRefreshList] = useState(true);

  const debouncedValue = useDebounce<ISearchModel>(searchVals, 1000);

  function setDefaultSort() {
    setSortBy(SortByEnum.score);
    setSortByScoreDesc(true);
    setSortByCvDateDesc(false);
  }

  useEffect(() => {
    setDefaultSort();
    onSearch(searchVals);
  }, [debouncedValue]);

  useEffect(() => {
    if (extSearch) {
      setSearchVals(extSearch);
    }
  }, [extSearch]);

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
    setDefaultSort();
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

      setSortByScoreDesc(false);
      setSortBy(SortByEnum.cvDate);
      onSort && onSort(SortByEnum.cvDate, sortDir);
    } else {
      if (sortByScoreDesc) {
        sortDir = "asc";
      } else {
        sortDir = "desc";
      }
      setSortByCvDateDesc(false);
      setSortBy(SortByEnum.score);
      onSort && onSort(SortByEnum.score, sortDir);
    }
  };

  return (
    <Stack
      onClick={(event) => {
        event.stopPropagation();
        event.preventDefault();
      }}
    >
      <Stack direction="row" alignItems={"center"} sx={{ direction: "rtl" }}>
        {showRefreshList && (
          <ToggleButtonGroup
            sx={{
              direction: "ltr",
              marginLeft: 0.5,
              "& .MuiButtonBase-root": {
                padding: "5px 3px",
                fontSize: "1.0rem",
              },
              "&.MuiToggleButtonGroup-root": {
                marginRight: "4px",
              },
            }}
            color="primary"
            value={refreshList}
            exclusive
            size="small"
            onChange={(event) => {
              event.stopPropagation();
              event.preventDefault();
              setRefreshList(!refreshList);
              setSearchVals((currentProps) => ({
                ...currentProps,
                value: "",
                advancedValue: "",
              }));
              onRefreshLists && onRefreshLists();
            }}
            aria-label="Platform"
          >
            <ToggleButton value={true} title="Load new cv's">
              <MdRefresh />
            </ToggleButton>
          </ToggleButtonGroup>
        )}
        <Search sx={{ direction: "rtl" }}>
          <SearchIconWrapper>
            <MdOutlineSearch
              onClick={(event) => {
                event.stopPropagation();
                event.preventDefault();
                search();
              }}
            />
          </SearchIconWrapper>
          <StyledInputBase
            placeholder="Search…"
            value={searchVals.value || ""}
            onChange={handleValueChange}
            onKeyDown={handleKeyDown}
          />

          {searchVals.value && (
            <IconWrapper sx={{ padding: "0 4px" }}>
              <MdOutlineClose
                onClick={() => {
                  setSearchVals((prevState) => ({
                    ...prevState,
                    value: "",
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
                    setSearchVals((prevState) => ({
                      ...prevState,
                      advancedValue: "",
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
        {showSortLeft && (
          <ToggleButtonGroup
            sx={{
              direction: "ltr",
              "& .MuiButtonBase-root": {
                padding: "5px 3px",
                fontSize: "1.0rem",
              },
            }}
            color={sortByLeft ? "primary" : "secondary"}
            value={sortByLeft}
            exclusive
            size="small"
            onChange={(event) => {
              event.stopPropagation();
              event.preventDefault();
              setSortByLeft(!sortByLeft);
              onSortLeftLists && onSortLeftLists(sortByLeft ? "asc" : "desc");
            }}
            aria-label="Platform"
          >
            <ToggleButton value={true} title="Sort by updated date">
              <MdSort />
            </ToggleButton>
          </ToggleButtonGroup>
        )}
        {((searchVals.value && records !== undefined) ||
          (!shoeAdvancedIcon && records !== undefined)) && (
          <div
            title={
              records === 300 ? "More then 300 records found" : "Records found"
            }
            style={{
              whiteSpace: "nowrap",
              direction: "ltr",
              padding: "0 3px",
              fontSize: "0.7rem",
            }}
          >
            {records === 300 ? `300...` : `${records} `}
          </div>
        )}
        {shoeAdvancedIcon && (
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
            <ToggleButton
              value={SortByEnum.cvDate}
              title="Sort by cv sent date"
            >
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
            <ToggleButton value={SortByEnum.score} title="Sort by score">
              <MdRule />
            </ToggleButton>
          </ToggleButtonGroup>
        </Stack>
      )}
    </Stack>
  );
};
