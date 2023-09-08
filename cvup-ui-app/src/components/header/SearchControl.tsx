import { InputBase, Stack, ToggleButton } from "@mui/material";
import { styled } from "@mui/material/styles";
import { useEffect, useState } from "react";
import {
  MdKeyboardArrowDown,
  MdKeyboardArrowUp,
  MdOutlineClose,
  MdOutlineSearch,
  MdOutlineTranslate,
  MdRefresh,
  MdSort,
} from "react-icons/md";
import useDebounce from "../../Hooks/useDebounce";
import { ISearchModel } from "../../models/GeneralModels";
import { SortByEnum } from "../../models/GeneralEnums";
import { translate } from "../../utils/GeneralUtils";
import { useStore } from "../../Hooks/useStore";

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
  onSort?: (isDesc: boolean) => void;
  showSort?: boolean;
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
  showSort = false,
  showRefreshList = false,
  onRefreshLists,
  extSearch,
}: IProps) => {
  const { generalStore } = useStore();

  const [showAdvancedSearch, setShowAdvancedSearch] = useState(false);
  const [searchVals, setSearchVals] = useState<ISearchModel>({
    value: "",
    exact: true,
    advancedValue: "",
  });
  const [sortAsc, setSortAsc] = useState(false);
  const [refreshList, setRefreshList] = useState(true);
  const [isLoaded, setIsLoaded] = useState(false);

  const debouncedValue = useDebounce<ISearchModel>(searchVals, 1000);

  useEffect(() => {
    if (isLoaded) {
      onSearch(searchVals);
    } else {
      setIsLoaded(true);
    }
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
    onSearch(searchVals);
  };

  return (
    <Stack
      onClick={(event) => {
        event.stopPropagation();
        event.preventDefault();
      }}
      sx={{ width: "100%", paddingRight: "3px" }}
    >
      <Stack
        direction="row"
        alignItems={"center"}
        sx={{ direction: "rtl", width: "100%" }}
      >
        {showRefreshList && (
          <ToggleButton
            sx={{
              direction: "ltr",
              "&.MuiButtonBase-root": {
                padding: "5px 3px",
                fontSize: "1.1rem",
              },
            }}
            value="check"
            color="primary"
            selected={true}
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
          >
            <MdRefresh />
          </ToggleButton>
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

                    if (searchVals.advancedValue) {
                      setSearchVals((prevState) => ({
                        ...prevState,
                        advancedValue: "",
                        exact: false,
                      }));
                    }
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
        {/* <ToggleButton
          sx={{
            direction: "ltr",
            "&.MuiButtonBase-root": {
              padding: "7px 5px",
              fontSize: "0.8rem",
            },
          }}
          value="check"
          color="primary"
          selected={false}
          onChange={async (event) => {
            const transList: string[] = [];

            let advEn = "";

            if (searchVals.value) {
              transList.push(searchVals.value);

              //const valEn = await translate(searchVals.value);

              if (searchVals.advancedValue) {
                transList.push(searchVals.advancedValue);

                //advEn = await translate(searchVals.advancedValue);
              }

              const res = await generalStore.translateMultiLines(
                transList,
                "en"
              );

              if (res.length) {
                setSearchVals((prevState) => ({
                  ...prevState,
                  value: res[0],
                  advancedValue: res.length === 2 ? res[1] : "",
                }));
              }
              
            }
          }}
        >
          <MdOutlineTranslate />
        </ToggleButton> */}
        {shoeAdvancedIcon && (
          <ToggleButton
            sx={{
              direction: "ltr",
              "&.MuiButtonBase-root": {
                padding: "2px 3px",
                fontSize: "0.8rem",
              },
            }}
            value="check"
            color="primary"
            selected={searchVals.exact}
            onChange={(event) => {
              event.stopPropagation();
              event.preventDefault();
              setSearchVals((currentProps) => ({
                ...currentProps,
                exact: !searchVals.exact,
              }));
            }}
          >
            Ex
          </ToggleButton>
        )}
        {showSort && (
          <ToggleButton
            sx={{
              direction: "ltr",
              "&.MuiButtonBase-root": {
                padding: "5px 2px",
                fontSize: "1.1rem",
              },
            }}
            title="Sort by date"
            value="check"
            color="primary"
            selected={sortAsc}
            onChange={(event) => {
              event.stopPropagation();
              event.preventDefault();
              setSortAsc(!sortAsc);
              onSort && onSort(sortAsc);
            }}
          >
            <MdSort />
          </ToggleButton>
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
        </Stack>
      )}
    </Stack>
  );
};
