import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { SessionComponent } from './session/session.component';
import { WeightSessionComponent } from './session/weight-session/weight-session.component';
import { TimeSessionComponent } from './session/time-session/time-session.component';

@NgModule({
  declarations: [
    AppComponent,
    SessionComponent,
    WeightSessionComponent,
    TimeSessionComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
