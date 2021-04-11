import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { CreateSessionComponent } from './create-session/create-session.component';
import { SurveySessionComponent } from './survey-session/survey-session.component';

@NgModule({
  declarations: [
    AppComponent,
    CreateSessionComponent,
    SurveySessionComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forRoot([
      { path: 'PollSession/:id/Username/:username', component: SurveySessionComponent, pathMatch: 'full' },
      { path: 'NewPoll', component: CreateSessionComponent},
      { path: '**', redirectTo: 'NewPoll' }
    ])
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
