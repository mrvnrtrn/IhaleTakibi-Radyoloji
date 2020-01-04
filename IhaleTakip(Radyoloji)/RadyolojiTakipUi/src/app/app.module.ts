import { BrowserModule } from '@angular/platform-browser';
import { NgModule, LOCALE_ID } from '@angular/core';

import { RadyolojiIslemService } from './entity/service/radyoloji-islem.service';
import { HttpClientModule } from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { TopMenuComponent } from './top-menu/top-menu.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {
  MatInputModule, MatPaginatorModule, MatProgressSpinnerModule,
  MatSortModule, MatTableModule, MatDialogModule
} from "@angular/material";
import { RadyolojiKayitComponent } from './radyoloji-modulu/radyoloji-kayit/radyoloji-kayit.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon'
import { MatButtonModule } from '@angular/material/button';
import { MomentModule } from 'ngx-moment';
import { DataTablesModule } from 'angular-datatables';
import { LoginComponent } from './login/login.component';
import { LogoutComponent } from './logout/logout.component';
import { HomeComponent } from './home/home.component';

@NgModule({
  declarations: [
    AppComponent,
    TopMenuComponent,
    RadyolojiKayitComponent,
    LoginComponent,
    LogoutComponent,
    HomeComponent,
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    MatInputModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatProgressSpinnerModule,
    HttpClientModule,
    AppRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    MatIconModule,
    MatButtonModule,
    MatDialogModule,
    MomentModule,
    MomentModule.forRoot({
      relativeTimeThresholdOptions: {
        'm': 59
      }
    }),
    DataTablesModule
    
  ],
  providers: [
    {provide: LOCALE_ID, useValue: "tr-TR"},
    RadyolojiIslemService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
