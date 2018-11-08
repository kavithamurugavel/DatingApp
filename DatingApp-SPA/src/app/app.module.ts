import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import {HttpClientModule} from '@angular/common/http';

import { AppComponent } from './app.component';
import { ValueComponent } from './value/value.component';

// this is where all the new components have to be declared
// (right click -> Generate Component automatically adds the new component here)
@NgModule({
   declarations: [
      AppComponent,
      ValueComponent
   ],
   imports: [
      BrowserModule,
      HttpClientModule // for http requests
   ],
   providers: [],
   bootstrap: [
      AppComponent
   ]
})
export class AppModule { }
