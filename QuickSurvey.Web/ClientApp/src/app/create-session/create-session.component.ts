import { HttpErrorResponse } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { FormArray, FormBuilder, FormControl, ValidatorFn, Validators } from '@angular/forms';
import { WINDOW } from '../injection-tokens';
import { SessionRequest } from '../models';
import { ApiService } from '../services/api.service';
import { AuthService } from '../services/auth.service';
import { SignalRService } from '../services/signalr.service';

const defaultValidation = ['', Validators.compose([Validators.required, Validators.minLength(5), Validators.maxLength(250)])]

@Component({
  selector: 'app-create-session',
  templateUrl: './create-session.component.html',
  providers: [AuthService, SignalRService, ApiService]
})
export class CreateSessionComponent {
  public static MAX_CHOICE = 6;

  public surveySessionForm = this.fb.group({
    title: defaultValidation,
    choices: this.fb.array([defaultValidation, defaultValidation])
  })

  public error = '';
  public isSubmitting = false;

  get title(): FormControl { return this.surveySessionForm.get('title') as FormControl }
  get choices(): FormArray { return this.surveySessionForm.get('choices') as FormArray; }
  get maxChoice(): number { return CreateSessionComponent.MAX_CHOICE; }

  get hasChoicesErrors(): boolean {
    return this.choices.controls.some(f => f.errors);
  }

  constructor(private fb: FormBuilder, private apiService: ApiService, @Inject(WINDOW) private window: Window) {
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
    this.isSubmitting = true;
    if (!this.surveySessionForm.valid) {
      this.error = "Unexpected validation error";
      return;
    }
    const body = this.surveySessionForm.value as SessionRequest;
    this.apiService.createSession(body).subscribe(url => {
      if (url == null) {
        this.error = "Unexpected network error";
      } else {
        this.window.location.href = url;
      }
    }, error => {
      if (error instanceof HttpErrorResponse) {
        this.error = error.message;
      }
      console.log(error)
      this.isSubmitting = false;
    })
  }
}

