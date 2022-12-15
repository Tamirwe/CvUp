import { List, ListItem, ListItemText, Stack, IconButton } from "@mui/material";
import { useEffect } from "react";
import { useStore } from "../../Hooks/useStore";
import { MdOutlineDelete, MdOutlineEdit } from "react-icons/md";
import { IIdName } from "../../models/AuthModels";
import { observer } from "mobx-react";

interface IProps {
  onAddEdit: (department: IIdName) => void;
}

export const DepartmentsList = observer((props: IProps) => {
  const { generalStore } = useStore();

  useEffect(() => {
    generalStore.getCompanyDepartments();
  }, []);

  const handleEdit = (department: IIdName) => {
    props.onAddEdit(department);
  };

  const handleDelete = (id: number) => {};

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
          <ListItem key={item.id} sx={{ padding: "0" }}>
            <ListItemText>{item.name}</ListItemText>
            <Stack
              direction="row"
              justifyContent="space-between"
              alignItems="center"
              spacing={1}
            >
              <IconButton
                onClick={() => handleEdit({ id: item.id, name: item.name })}
                sx={{ color: "#cbe9b9" }}
              >
                <MdOutlineEdit />
              </IconButton>
              <IconButton
                onClick={() => handleDelete(item.id)}
                sx={{ color: "#e9b9bb" }}
              >
                <MdOutlineDelete />
              </IconButton>
            </Stack>
          </ListItem>
        );
      })}
    </List>
  );
});
