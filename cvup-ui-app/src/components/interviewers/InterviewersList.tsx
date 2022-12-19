import { List, ListItemText, IconButton, ListItemButton } from "@mui/material";
import { useEffect } from "react";
import { useStore } from "../../Hooks/useStore";
import { MdOutlineDelete } from "react-icons/md";
import { IInterviewer } from "../../models/AuthModels";
import { observer } from "mobx-react";
import { CrudTypesEnum } from "../../models/GeneralEnums";

interface IProps {
  onAddEditDeleteclick: (department: IInterviewer, type: CrudTypesEnum) => void;
}

export const InterviewersList = observer((props: IProps) => {
  const { authStore } = useStore();

  useEffect(() => {
    (async () => {
      await authStore.getInterviewers(false);
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
      {authStore.interviewersList?.map((item, i) => {
        return (
          <ListItemButton
            onClick={() =>
              props.onAddEditDeleteclick(item, CrudTypesEnum.Update)
            }
            key={item.id}
            sx={{ borderBottom: "1px solid #f1f1f1" }}
          >
            <ListItemText>{`${item.firstName} ${item.lastName}`}</ListItemText>
            {parseInt(authStore.claims.UserId || "0") !== item.id && (
              <IconButton
                onClick={(e) => {
                  e.stopPropagation();
                  e.preventDefault();
                  props.onAddEditDeleteclick(item, CrudTypesEnum.Delete);
                }}
                sx={{ color: "#d7d2d2" }}
              >
                <MdOutlineDelete />
              </IconButton>
            )}
          </ListItemButton>
        );
      })}
    </List>
  );
});
