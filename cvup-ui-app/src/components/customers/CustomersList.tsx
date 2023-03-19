import { List, ListItemText, ListItemButton } from "@mui/material";
import { useEffect } from "react";
import { useStore } from "../../Hooks/useStore";
import { IIdName } from "../../models/AuthModels";
import { observer } from "mobx-react";
import { CrudTypesEnum } from "../../models/GeneralEnums";

interface IProps {
  onCustomerClick: () => void;
}

export const CustomersList = observer((props: IProps) => {
  const { customersContactsStore } = useStore();

  useEffect(() => {
    (async () => {
      await customersContactsStore.getCustomersList(false);
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
      {customersContactsStore.customersList?.map((item, i) => {
        return (
          <ListItemButton
            onClick={() => {
              customersContactsStore.selectedCustomer = item;
              props.onCustomerClick();
            }}
            key={item.id}
            sx={{ borderBottom: "1px solid #f1f1f1" }}
          >
            <ListItemText>{item.name}</ListItemText>
          </ListItemButton>
        );
      })}
    </List>
  );
});
