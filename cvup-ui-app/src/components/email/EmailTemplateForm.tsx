import {
  Button,
  FormControl,
  FormHelperText,
  Grid,
  IconButton,
  InputLabel,
  Link,
  MenuItem,
  Select,
  Stack,
  TextField,
} from "@mui/material";
import { observer } from "mobx-react";
import { useEffect, useRef, useState } from "react";
import { useFormErrors } from "../../Hooks/useFormErrors";
import { useStore } from "../../Hooks/useStore";
import {
  AlertConfirmDialogEnum,
  DynamicEmailDataEnum,
  TextValidateTypeEnum,
} from "../../models/GeneralEnums";
import { IEmailTemplate } from "../../models/GeneralModels";
import { validateTxt } from "../../utils/Validation";
import { GoPlus } from "react-icons/go";
import { MdContentCopy } from "react-icons/md";
import { copyToClipBoard } from "../../utils/GeneralUtils";
import { QuillRte } from "../rte/QuillRte";

interface IProps {
  onSaved: () => void;
  onCancel: () => void;
}

export const EmailTemplateForm = observer(({ onSaved, onCancel }: IProps) => {
  const { candsStore, generalStore } = useStore();
  const refQuill = useRef();

  // const [emailTemplate, setEmailTemplate] = useState( null  );
  const [submitError, setSubmitError] = useState("");
  const [formModel, setFormModel] = useState<IEmailTemplate>({
    id: 0,
    name: "",
    subject: "",
    body: "",
  });
  const [updateFieldError, clearError, errModel] = useFormErrors({
    name: "",
    subject: "",
    // body: "",
  });

  useEffect(() => {
    if (candsStore.emailTemplates?.length) {
      setFormModel(candsStore.emailTemplates[0]);
    }
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  const validateForm = () => {
    let isFormValid = true,
      err = "";

    err = validateTxt(formModel.name || "", [
      TextValidateTypeEnum.notEmpty,
      TextValidateTypeEnum.twoCharsMin,
    ]);
    isFormValid = updateFieldError("name", err) && isFormValid;

    err = validateTxt(formModel.subject || "", [
      TextValidateTypeEnum.notEmpty,
      TextValidateTypeEnum.twoCharsMin,
    ]);
    isFormValid = updateFieldError("subject", err) && isFormValid;

    // err = validateTxt(formModel.body || "", [
    //   TextValidateTypeEnum.notEmpty,
    //   TextValidateTypeEnum.twoCharsMin,
    // ]);
    // isFormValid = updateFieldError("body", err) && isFormValid;

    return isFormValid;
  };

  const handleSubmit = async () => {
    const quillEditor = refQuill.current as any;
    //const reviewText = quillEditor.getText();
    formModel.body = quillEditor.root.innerHTML;

    if (validateForm()) {
      let response;

      response = await candsStore.addUpdateEmailTemplate(formModel);

      if (response.isSuccess) {
        onSaved();
      } else {
        return setSubmitError("An Error Occurred Please Try Again Later.");
      }
    }
  };

  const deletePosition = async () => {
    const isDelete = await generalStore.alertConfirmDialog(
      AlertConfirmDialogEnum.Confirm,
      "Delete Email Template",
      "Are you sure you want to delete this template?"
    );

    if (isDelete) {
      const response = await candsStore.deleteEmailTemplate(formModel.id);

      if (response.isSuccess) {
        onSaved();
      } else {
        return setSubmitError("An Error Occurred Please Try Again Later.");
      }
    }
  };

  const handleAddTemplate = async () => {
    setFormModel({
      id: 0,
      name: "",
      subject: "",
      body: "",
      stageToUpdate: "",
    });
  };

  return (
    <>
      {formModel && (
        <form noValidate spellCheck="false">
          <Grid container>
            <Grid item xs={12} lg={12} sx={{ direction: "rtl" }}>
              <Grid container>
                <Grid item xs={12} lg={12} pt={1}>
                  <Grid container>
                    <Grid item xs={12} lg={7}>
                      <Stack direction="row">
                        <IconButton color="primary" onClick={handleAddTemplate}>
                          <GoPlus />
                        </IconButton>

                        <TextField
                          sx={{
                            direction: "rtl",
                            display: formModel.id === 0 ? "block" : "none",
                          }}
                          fullWidth
                          required
                          margin="none"
                          type="text"
                          id="title"
                          label="Template Name"
                          variant="outlined"
                          onChange={(e) => {
                            setFormModel((currentProps) => ({
                              ...currentProps,
                              name: e.target.value,
                            }));
                            clearError("name");
                          }}
                          error={errModel.name !== ""}
                          helperText={errModel.name}
                          value={formModel.name}
                        />
                        <FormControl
                          fullWidth
                          sx={{
                            direction: "rtl",
                            display: formModel.id === 0 ? "none" : "block",
                          }}
                        >
                          <InputLabel id="emailTemplatelabel">
                            Email Template
                          </InputLabel>
                          <Select
                            fullWidth
                            labelId="emailTemplate"
                            id="emailTemplate"
                            label="Email Template"
                            onChange={(e) => {
                              const template = candsStore.emailTemplates?.find(
                                (x) => x.id === e.target.value
                              );

                              if (template) {
                                setFormModel(template);
                              }
                            }}
                            value={formModel.id}
                          >
                            {candsStore.emailTemplates?.map((item) => {
                              return (
                                <MenuItem
                                  key={item.id}
                                  value={item.id}
                                  sx={{ direction: "rtl" }}
                                >
                                  {item.name}
                                </MenuItem>
                              );
                            })}
                          </Select>
                        </FormControl>
                      </Stack>
                    </Grid>

                    <Grid item xs={12} lg={5}>
                      <FormControl
                        fullWidth
                        // variant="standard"
                        // sx={{ minWidth: 250 }}
                      >
                        <InputLabel id="stageSelectlabel">
                          Candidate status after send
                        </InputLabel>
                        <Select
                          sx={{
                            direction: "ltr",
                            "& .MuiSelect-select": {
                              color: candsStore.posStages?.find(
                                (x) => x.stageType === formModel.stageToUpdate
                              )?.color,
                              fontWeight: "bold",
                            },
                          }}
                          id="stageSelect"
                          label="Candidate status after send"
                          value={formModel.stageToUpdate || ""}
                          onChange={async (e) => {
                            setFormModel((currentProps) => ({
                              ...currentProps,
                              stageToUpdate: e.target.value,
                            }));
                          }}
                        >
                          <MenuItem value="" key="0"></MenuItem>
                          {candsStore.posStages?.map((item, ind) => {
                            // console.log(key, index);
                            return (
                              <MenuItem
                                sx={{ color: item.color }}
                                key={ind}
                                value={item.stageType}
                              >
                                {item.name}
                              </MenuItem>
                            );
                          })}
                        </Select>
                      </FormControl>
                    </Grid>
                  </Grid>
                </Grid>
                <Grid item xs={12} lg={12}>
                  <TextField
                    sx={{
                      direction: "rtl",
                    }}
                    fullWidth
                    required
                    margin="normal"
                    type="text"
                    id="subject"
                    label="Subject"
                    variant="outlined"
                    onChange={(e) => {
                      setFormModel((currentProps) => ({
                        ...currentProps,
                        subject: e.target.value,
                      }));
                      clearError("subject");
                    }}
                    error={errModel.subject !== ""}
                    helperText={errModel.subject}
                    value={formModel.subject}
                  />
                </Grid>
                <Grid
                  item
                  xs={12}
                  lg={12}
                  pt={1}
                  sx={{
                    direction: "ltr",
                  }}
                >
                  <QuillRte ref={refQuill} quillHtml={formModel.body} />
                </Grid>
                {/* <Grid item xs={12} lg={12}>
                  <TextField
                    sx={{
                      direction: "rtl",
                    }}
                    fullWidth
                    multiline
                    rows={10}
                    required
                    margin="normal"
                    type="text"
                    id="body"
                    label="body"
                    variant="outlined"
                    onChange={(e) => {
                      setFormModel((currentProps) => ({
                        ...currentProps,
                        body: e.target.value,
                      }));
                      clearError("body");
                      setIsDirty(true);
                    }}
                    error={errModel.body !== ""}
                    helperText={errModel.body}
                    value={formModel.body}
                  />
                </Grid> */}
              </Grid>
              <Grid container gap={1} style={{ direction: "ltr" }}>
                <Grid item xs={12} lg={12} sx={{ direction: "ltr" }}>
                  <div
                    style={{
                      textDecoration: "underline",
                      paddingBottom: 4,
                      paddingTop: 4,
                    }}
                  >
                    Dynamic data to add:
                  </div>
                </Grid>
                {Object.keys(DynamicEmailDataEnum)
                  .filter((x) => isNaN(Number(x)))
                  .map((key, i) => {
                    return (
                      <Grid item xs="auto">
                        <Link
                          href="#"
                          onClick={() => {
                            copyToClipBoard(`[${key}]`);
                          }}
                        >
                          <MdContentCopy />
                          {`[${key}]`}
                        </Link>
                      </Grid>
                    );
                  })}
              </Grid>

              {submitError && (
                <Grid item xs={12}>
                  <div style={{ direction: "ltr" }}>
                    <FormHelperText error>{submitError}</FormHelperText>
                  </div>
                </Grid>
              )}
              <Grid item xs={12} mt={4}>
                <Grid container justifyContent="space-between">
                  <Grid item>
                    <Stack direction="row" alignItems="center" gap={1}>
                      <Button
                        fullWidth
                        color="error"
                        onClick={() => {
                          deletePosition();
                        }}
                      >
                        Delete
                      </Button>
                    </Stack>
                  </Grid>
                  <Grid item>
                    <Stack direction="row" alignItems="center" gap={1}>
                      <Button
                        fullWidth
                        color="secondary"
                        onClick={() => onCancel()}
                      >
                        Cancel
                      </Button>

                      <Button
                        fullWidth
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
          </Grid>
        </form>
      )}
    </>
  );
});
