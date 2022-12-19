import {
  List,
  ListItemText,
  Stack,
  IconButton,
  ListItemButton,
} from "@mui/material";
import { useEffect } from "react";
import { useStore } from "../../Hooks/useStore";
import { MdOutlineDelete } from "react-icons/md";
import { IInterviewer } from "../../models/AuthModels";
import { observer } from "mobx-react";
import { CrudTypes } from "../../models/GeneralEnums";

interface IProps {
  onAddEditDeleteclick: (department: IInterviewer, type: CrudTypes) => void;
}

export const InterviewersList = observer((props: IProps) => {
  const { authStore } = useStore();

  useEffect(() => {
    (async () => {
      await authStore.getInterviewers(false);
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
      {authStore.interviewersList?.map((item, i) => {
        return (
          <ListItemButton
            onClick={() => props.onAddEditDeleteclick(item, CrudTypes.Update)}
            key={item.id}
            sx={{ borderBottom: "1px solid #f1f1f1" }}
          >
            <ListItemText>{`${item.firstName} ${item.lastName}`}</ListItemText>
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
