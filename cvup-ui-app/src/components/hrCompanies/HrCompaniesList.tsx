import { List, ListItemText, IconButton, ListItemButton } from "@mui/material";
import { useEffect } from "react";
import { useStore } from "../../Hooks/useStore";
import { MdOutlineDelete } from "react-icons/md";
import { IIdName } from "../../models/AuthModels";
import { observer } from "mobx-react";
import { CrudTypes } from "../../models/GeneralEnums";

interface IProps {
  onAddEditDeleteclick: (hrCompany: IIdName, type: CrudTypes) => void;
}

export const HrCompaniesList = observer((props: IProps) => {
  const { generalStore } = useStore();

  useEffect(() => {
    (async () => {
      await generalStore.getHrCompanies(false);
    })();
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  return (
    <List
      sx={{
        width: "100%",
        bgcolor: "background.paper",
        position: "relative",
        overflow: "auto",
        maxHeight: 300,
      }}
    >
      {generalStore.hrCompaniesList?.map((item, i) => {
        return (
          <ListItemButton
            onClick={() => props.onAddEditDeleteclick(item, CrudTypes.Update)}
            key={item.id}
            sx={{ borderBottom: "1px solid #f1f1f1" }}
          >
            <ListItemText>{item.name}</ListItemText>
            <IconButton
              onClick={(e) => {
                e.stopPropagation();
                e.preventDefault();
                props.onAddEditDeleteclick(item, CrudTypes.Delete);
              }}
              sx={{ color: "#d7d2d2" }}
            >
              <MdOutlineDelete />
            </IconButton>
          </ListItemButton>
        );
      })}
    </List>
  );
});
