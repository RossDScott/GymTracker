import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { SessionComponent } from './session/session.component';
import { WeightSessionComponent } from './session/weight-session/weight-session.component';

@NgModule({
  declarations: [
    AppComponent,
    SessionComponent,
    WeightSessionComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
