import {
  alpha,
  Box,
  Button,
  Dialog,
  DialogContent,
  DialogTitle,
  Divider,
  FormHelperText,
  Grid,
  IconButton,
  List,
  ListItemButton,
  ListItemText,
  Stack,
  TextField,
  Typography,
  useMediaQuery,
  useTheme,
} from "@mui/material";
import { observer } from "mobx-react";
import { useEffect, useRef, useState } from "react";
import { GoPlus } from "react-icons/go";
import { MdClose } from "react-icons/md";
import { format } from "date-fns";
import { useStore } from "../../Hooks/useStore";
import { useFormErrors } from "../../Hooks/useFormErrors";
import {
  AlertConfirmDialogEnum,
  TextValidateTypeEnum,
} from "../../models/GeneralEnums";
import { ICustomer } from "../../models/GeneralModels";
import { validateTxt } from "../../utils/Validation";
import { CustomerContacts } from "./CustomerContacts";

interface IProps {
  isOpen: boolean;
  onClose: () => void;
}

const emptyCustomer: ICustomer = {
  id: 0,
  name: "",
  descr: "",
  address: "",
};

export const CustomerDialog = observer(({ isOpen, onClose }: IProps) => {
  const { customersContactsStore, generalStore } = useStore();
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down("sm"));
  const formRef = useRef<HTMLDivElement>(null);
  const [searchTxt, setSearchTxt] = useState("");
  const [isDirty, setIsDirty] = useState(false);
  const [submitError, setSubmitError] = useState("");
  const [formModel, setFormModel] = useState<ICustomer>({ ...emptyCustomer });
  const [updateFieldError, clearError, errModel] = useFormErrors({
    name: "",
  });

  const isNew = formModel.id === 0;

  const dataList = (customersContactsStore.customersList as ICustomer[])
    .filter(
      (x) =>
        !searchTxt || x.name.toLowerCase().includes(searchTxt.toLowerCase())
    )
    .slice()
    .sort((a, b) => a.name.localeCompare(b.name));

  useEffect(() => {
    if (customersContactsStore.selectedCustomer) {
      selectCustomer(customersContactsStore.selectedCustomer as ICustomer);
    }
    if (customersContactsStore.contactsList.length === 0) {
      customersContactsStore.getContactsList();
    }
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  const resetFormState = () => {
    setIsDirty(false);
    setSubmitError("");
    clearError("name");
  };

  const selectCustomer = (customer: ICustomer) => {
    customersContactsStore.selectedCustomer = customer;
    setFormModel({ ...emptyCustomer, ...customer });
    resetFormState();
  };

  const onCustomerClick = (customer: ICustomer) => {
    selectCustomer(customer);
    if (isMobile) {
      formRef.current?.scrollIntoView({ behavior: "smooth", block: "start" });
    }
  };

  const newCustomer = () => {
    customersContactsStore.selectedCustomer = undefined;
    setFormModel({ ...emptyCustomer });
    resetFormState();
  };

  const updateField = (field: keyof ICustomer, value: string) => {
    setFormModel((currentProps) => ({
      ...currentProps,
      [field]: value,
    }));
    setIsDirty(true);
  };

  const validateForm = () => {
    const err = validateTxt(formModel.name, [
      TextValidateTypeEnum.notEmpty,
      TextValidateTypeEnum.twoCharsMin,
      TextValidateTypeEnum.startWithTwoLetters,
    ]);

    return updateFieldError("name", err);
  };

  const handleSubmit = async () => {
    setIsDirty(false);
    if (!validateForm()) return;

    const response = isNew
      ? await customersContactsStore.addCustomer(formModel)
      : await customersContactsStore.updateCustomer(formModel);

    if (response.isSuccess) {
      customersContactsStore.setCustomerAddedUpdated(response.data);
      await customersContactsStore.getCustomersList();

      const savedId = response.data?.id || formModel.id;
      const saved = (customersContactsStore.customersList as ICustomer[]).find(
        (x) => x.id === savedId
      );
      saved ? selectCustomer(saved) : newCustomer();
    } else {
      setSubmitError("An Error Occurred Please Try Again Later.");
    }
  };

  const deleteRecord = async () => {
    const isDelete = await generalStore.alertConfirmDialog(
      AlertConfirmDialogEnum.Confirm,
      "Delete Customer",
      "Are you sure you want to delete this customer?"
    );

    if (!isDelete) return;

    const response = await customersContactsStore.deleteCustomer(formModel.id);

    if (response.isSuccess) {
      if (formModel.id === customersContactsStore.selectedContact?.customerId) {
        generalStore.showContactFormDialog = false;
        customersContactsStore.getContactsList();
      }

      await customersContactsStore.getCustomersList();
      newCustomer();
    } else {
      setSubmitError("An Error Occurred Please Try Again Later.");
    }
  };

  return (
    <Dialog
      open={isOpen}
      onClose={onClose}
      fullWidth
      maxWidth={"md"}
      fullScreen={isMobile}
    >
      <DialogTitle sx={{ position: "relative", pr: 6 }}>
        Customers
        <Button
          onClick={() => {
            newCustomer();
            if (isMobile) {
              formRef.current?.scrollIntoView({
                behavior: "smooth",
                block: "start",
              });
            }
          }}
          startIcon={<GoPlus />}
          sx={{ ml: 2 }}
        >
          {isMobile ? "Add New" : "Add New Customer"}
        </Button>
        {customersContactsStore.selectedCustomer && (
          <Typography
            variant="h6"
            component="span"
            noWrap
            sx={{
              position: { xs: "static", sm: "absolute" },
              left: "50%",
              top: "50%",
              transform: { xs: "none", sm: "translate(-50%, -50%)" },
              display: { xs: "block", sm: "inline" },
              width: { xs: "fit-content", sm: "auto" },
              maxWidth: { xs: "100%", sm: "30%" },
              mt: { xs: 1, sm: 0 },
              fontWeight: 700,
              fontSize: "1.35rem",
              color: "primary.main",
              bgcolor: (theme) => alpha(theme.palette.primary.main, 0.08),
              border: 1,
              borderColor: (theme) => alpha(theme.palette.primary.main, 0.3),
              borderRadius: 2,
              px: 2,
              py: 0.5,
            }}
          >
            {customersContactsStore.selectedCustomer.name}
          </Typography>
        )}
        <IconButton
          aria-label="close"
          onClick={onClose}
          sx={{
            position: "absolute",
            right: 8,
            top: 8,
            color: (theme) => theme.palette.grey[500],
          }}
        >
          <MdClose />
        </IconButton>
      </DialogTitle>
      <DialogContent>
        <Stack
          direction={{ xs: "column", sm: "row" }}
          gap={{ xs: 1, sm: 3 }}
          sx={{ minHeight: { xs: 0, sm: 480 } }}
        >
          <Box sx={{ width: { xs: "100%", sm: 280 }, flexShrink: 0 }}>
            <TextField
              fullWidth
              margin="normal"
              type="text"
              id="search"
              label="Search Customer"
              variant="outlined"
              onChange={(e) => setSearchTxt(e.target.value)}
              value={searchTxt}
            />
            <List
              sx={{
                width: "100%",
                bgcolor: "background.paper",
                overflow: "auto",
                maxHeight: { xs: 200, sm: 400 },
              }}
            >
              {dataList.map((item) => (
                <ListItemButton
                  key={item.id}
                  selected={item.id === formModel.id}
                  onClick={() => onCustomerClick(item)}
                  sx={{ borderBottom: "1px solid #f1f1f1" }}
                >
                  <ListItemText>{item.name}</ListItemText>
                </ListItemButton>
              ))}
            </List>
          </Box>

          <Divider
            orientation={isMobile ? "horizontal" : "vertical"}
            flexItem
          />

          <Box ref={formRef} sx={{ flexGrow: 1, pt: { xs: 1, sm: 2 } }}>
            <Box
              sx={{
                border: 1,
                borderColor: "divider",
                borderRadius: 2,
                p: { xs: 1.5, sm: 2 },
              }}
            >
              <Stack
                direction="row"
                alignItems="center"
                justifyContent="space-between"
              >
                <Typography variant="subtitle1">
                  {isNew ? "Add Customer" : "Edit Customer"}
                </Typography>
                {!isNew && formModel.created && (
                  <Typography
                    variant="body2"
                    color="text.secondary"
                    title="Created"
                  >
                    {format(new Date(formModel.created), "MMM d, yyyy")}
                  </Typography>
                )}
              </Stack>
              <form noValidate spellCheck="false">
                <Grid container>
                  <Grid item xs={12}>
                    <TextField
                      fullWidth
                      autoFocus={!isMobile}
                      margin="normal"
                      type="text"
                      id="name"
                      label="Name"
                      variant="outlined"
                      onChange={(e) => {
                        updateField("name", e.target.value);
                        clearError("name");
                      }}
                      error={errModel.name !== ""}
                      helperText={errModel.name}
                      value={formModel.name}
                    />
                  </Grid>
                  <Grid item xs={12}>
                    <TextField
                      sx={{ direction: "rtl" }}
                      inputProps={{ maxLength: 255 }}
                      fullWidth
                      margin="normal"
                      type="text"
                      id="address"
                      label="Address"
                      variant="outlined"
                      onChange={(e) => updateField("address", e.target.value)}
                      value={formModel.address || ""}
                    />
                  </Grid>
                  <Grid item xs={12}>
                    <TextField
                      sx={{ direction: "rtl" }}
                      multiline
                      rows={5}
                      inputProps={{ maxLength: 255 }}
                      fullWidth
                      margin="normal"
                      type="text"
                      id="descr"
                      label="Description"
                      variant="outlined"
                      onChange={(e) => updateField("descr", e.target.value)}
                      value={formModel.descr || ""}
                    />
                  </Grid>
                  <Grid item xs={12}>
                    <FormHelperText error>{submitError}</FormHelperText>
                  </Grid>
                  <Grid item xs={12} mt={2}>
                    <Grid container justifyContent="space-between">
                      <Grid item>
                        {!isNew && (
                          <Button color="warning" onClick={deleteRecord}>
                            Delete
                          </Button>
                        )}
                      </Grid>
                      <Grid item>
                        <Stack direction="row" alignItems="center" gap={1}>
                          <Button color="secondary" onClick={onClose}>
                            Cancel
                          </Button>
                          <Button
                            disabled={!isDirty}
                            color="secondary"
                            onClick={handleSubmit}
                          >
                            Save
                          </Button>
                        </Stack>
                      </Grid>
                    </Grid>
                  </Grid>
                </Grid>
              </form>
            </Box>
            {!isNew && (
              <CustomerContacts
                key={formModel.id}
                customerId={formModel.id}
                customerName={formModel.name}
              />
            )}
          </Box>
        </Stack>
      </DialogContent>
    </Dialog>
  );
});
