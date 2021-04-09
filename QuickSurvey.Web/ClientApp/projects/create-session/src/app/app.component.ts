import { Component, OnChanges, SimpleChanges } from '@angular/core';
import { FormArray, FormBuilder, FormControl, ValidatorFn, Validators } from '@angular/forms';

const defaultValidation = ['', Validators.compose([Validators.required, Validators.minLength(5), Validators.maxLength(250)])]

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
})
export class AppComponent {
  public static MAX_CHOICE = 6;

  public surveySessionForm = this.fb.group({
    title: defaultValidation,
    choices: this.fb.array([defaultValidation, defaultValidation])
  })

  public error = '';

  get title(): FormControl { return this.surveySessionForm.get('title') as FormControl }
  get choices(): FormArray { return this.surveySessionForm.get('choices') as FormArray; }
  get maxChoice(): number { return AppComponent.MAX_CHOICE; }

  get hasChoicesErrors(): boolean {
    return this.choices.controls.some(f => f.errors);
  }

  constructor(private fb: FormBuilder) {
    this.choices.errors
  }

  public addChoice() {
    this.choices.push(this.fb.control(defaultValidation[0], defaultValidation[1] as ValidatorFn));
    console.log(this.choices.errors);
    this.title.errors
  }

  public removeChoice(): void {
    this.choices.removeAt(-1);
  }

  public onSubmit(): void {
    if (!this.surveySessionForm.valid) {
      this.error = "Unexpected validation error";
      return;
    }
    const body = this.surveySessionForm.value as {title: string, choices: string[]};
    console.log(JSON.stringify(body, null, 2));
  }
}

