import { Box, Drawer, Grid } from "@mui/material";
import { Outlet } from "react-router-dom";
import { Header } from "../components/header/Header";
import { LeftListsWrapper } from "./LeftListsWrapper";
import { observer } from "mobx-react";
import { CandsListsWrapper } from "./CandsListsWrapper";
import { CandReview } from "../components/cands/CandReview";
import { useStore } from "../Hooks/useStore";
import { CrudTypesEnum, EmailTypeEnum } from "../models/GeneralEnums";
import { CandidateEmailSender } from "../components/email/CandidateEmailSender";
import { ContactEmailSender } from "../components/email/ContactEmailSender";
import { ContactsFormDialog } from "../components/contacts/ContactsFormDialog";
import { FormFolderDialog } from "../components/folders/FormFolderDialog";
import { useState } from "react";
import { AlertDialog } from "./AlertDialog";
import { UsersFormDialog } from "../components/users/UsersFormDialog";

export const LayoutAuth = observer(() => {
  const { generalStore } = useStore();
  return (
    <Box sx={{ flexGrow: 1 }}>
      {generalStore.cvReviewDialogOpen && <CandReview />}
      {generalStore.showEmailDialog === EmailTypeEnum.Candidate && (
        <CandidateEmailSender
          onClose={() => (generalStore.showEmailDialog = EmailTypeEnum.None)}
          open={generalStore.showEmailDialog === EmailTypeEnum.Candidate}
        />
      )}
      {generalStore.showEmailDialog === EmailTypeEnum.Contact && (
        <ContactEmailSender
          onClose={() => (generalStore.showEmailDialog = EmailTypeEnum.None)}
          open={generalStore.showEmailDialog === EmailTypeEnum.Contact}
        />
      )}
      {(generalStore.openModeFolderFormDialog as CrudTypesEnum) !==
        CrudTypesEnum.None && (
        <FormFolderDialog
          isOpen={
            (generalStore.openModeFolderFormDialog as CrudTypesEnum) !==
            CrudTypesEnum.None
          }
          onClose={() =>
            (generalStore.openModeFolderFormDialog = CrudTypesEnum.None)
          }
        />
      )}
      {generalStore.showContactFormDialog && (
        <ContactsFormDialog
          crudType={CrudTypesEnum.Insert}
          isOpen={generalStore.showContactFormDialog}
          onClose={() => (generalStore.showContactFormDialog = false)}
        />
      )}
      {generalStore.showUserFormDialog && (
        <UsersFormDialog
          crudType={CrudTypesEnum.Insert}
          isOpen={generalStore.showUserFormDialog}
          onClose={() => (generalStore.showUserFormDialog = false)}
        />
      )}
      {generalStore.confirmDialogOpen && <AlertDialog />}
      <Grid container spacing={0} columns={18}>
        <Grid item xs={5}>
          <Drawer
            open={true}
            variant="persistent"
            anchor="left"
            sx={{
              [`& .MuiDrawer-paper`]: {
                display: "contents",
              },
            }}
          >
            <LeftListsWrapper />
          </Drawer>
        </Grid>
        <Grid
          item
          xs={13}
          columns={12}
          sx={{ backgroundColor: "#fff", paddingLeft: "10px" }}
        >
          <Header />
          <Grid
            container
            spacing={0}
            sx={{ marginTop: "58px", borderRadius: "6px" }}
          >
            <Grid item xs={7}>
              <Outlet />
            </Grid>
            <Grid item xs={5}>
              <Drawer
                open={true}
                variant="persistent"
                anchor="right"
                sx={{
                  [`& .MuiDrawer-paper`]: {
                    display: "contents",
                  },
                }}
              >
                <CandsListsWrapper />
              </Drawer>
            </Grid>
          </Grid>
        </Grid>
      </Grid>
    </Box>
  );
});
