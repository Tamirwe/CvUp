import {
  List,
  ListItem,
  ListItemText,
  Stack,
  IconButton,
  Divider,
  ListItemButton,
} from "@mui/material";
import { useEffect } from "react";
import { useStore } from "../../Hooks/useStore";
import { MdOutlineDelete, MdOutlineEdit } from "react-icons/md";
import { IIdName } from "../../models/AuthModels";
import { observer } from "mobx-react";
import { CrudTypes } from "../../models/GeneralEnums";

interface IProps {
  onAddEditDeleteclick: (department: IIdName, type: CrudTypes) => void;
}

export const InterviewersList = observer((props: IProps) => {
  const { generalStore } = useStore();

  useEffect(() => {
    (async () => {
      await generalStore.getDepartments(false);
    })();
  }, []);

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
      {generalStore.departmentsList?.map((item, i) => {
        return (
          <ListItemButton
            onClick={() => props.onAddEditDeleteclick(item, CrudTypes.Update)}
            key={item.id}
            sx={{ borderBottom: "1px solid #f1f1f1" }}
          >
            <ListItemText>{item.name}</ListItemText>
            <Stack
              direction="row"
              justifyContent="space-between"
              alignItems="center"
              spacing={1}
            >
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
            </Stack>
          </ListItemButton>
        );
      })}
    </List>
  );
});
