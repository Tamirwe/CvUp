import { List, ListItemText, IconButton, ListItemButton } from "@mui/material";
import { useEffect } from "react";
import { useStore } from "../../Hooks/useStore";
import { MdOutlineDelete, MdStar, MdStarOutline } from "react-icons/md";
import { observer } from "mobx-react";
import { ISearchModel } from "../../models/GeneralModels";

interface IProps {
  onApplySearch: (searchVals: ISearchModel) => void;
}

export const SearchesList = observer(({ onApplySearch }: IProps) => {
  const { candsStore } = useStore();

  useEffect(() => {}, []); // eslint-disable-line react-hooks/exhaustive-deps

  return (
    <List
      sx={{
        width: "100%",
        bgcolor: "background.paper",
        position: "relative",
        overflow: "auto",
        height: "calc(100vh - 187px)",
      }}
    >
      {candsStore.sortedSearchesList?.map((item, i) => {
        return (
          <ListItemButton
            key={item.id}
            sx={{ borderBottom: "1px solid #f1f1f1", textAlign: "right" }}
            onClick={() => {
              onApplySearch({
                value: item.value,
                advancedValue: item.advancedValue || "",
                exact: item.exact,
              });
            }}
          >
            <IconButton
              onClick={(e) => {
                e.stopPropagation();
                e.preventDefault();
                candsStore.deleteSearch(item);
              }}
              sx={{ color: "#d7d2d2" }}
            >
              <MdOutlineDelete />
            </IconButton>
            <IconButton
              title="Saved Searches"
              sx={{ color: "#d7d2d2" }}
              onClick={(e) => {
                e.stopPropagation();
                e.preventDefault();
                candsStore.starSearch(item);
              }}
            >
              {item.star ? (
                <MdStar style={{ color: "blue" }} />
              ) : (
                <MdStarOutline />
              )}
            </IconButton>
            <ListItemText secondary={item.advancedValue}>
              {item.value}
            </ListItemText>
          </ListItemButton>
        );
      })}
    </List>
  );
});
