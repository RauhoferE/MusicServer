import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './views/user/login/login.component';
import { RegisterComponent } from './views/user/register/register.component';
import { CommonModule, registerLocaleData } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';
import { PlaylistsComponent } from './views/playlist/playlists/playlists.component';
import { NotFoundComponent } from './views/common/not-found/not-found.component';
import { NZ_I18N } from 'ng-zorro-antd/i18n';
import { en_US } from 'ng-zorro-antd/i18n';
import en from '@angular/common/locales/en';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {NzMessageModule} from 'ng-zorro-antd/message';
import {NzInputModule } from 'ng-zorro-antd/input';
import {NzFormModule} from 'ng-zorro-antd/form';
import { NzIconModule } from 'ng-zorro-antd/icon';
import {NzButtonModule} from 'ng-zorro-antd/button';
import { NzGridModule } from 'ng-zorro-antd/grid';

registerLocaleData(en);

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegisterComponent,
    PlaylistsComponent,
    NotFoundComponent
  ],
  imports: [
    BrowserModule,
    RouterModule,
    CommonModule,
    AppRoutingModule,
    FormsModule,
    HttpClientModule,
    BrowserAnimationsModule,
    ReactiveFormsModule,

    // Ng-zorro
    NzMessageModule,
    NzInputModule,
    NzFormModule,
    NzIconModule,
    NzButtonModule,
    NzGridModule,
  ],
  providers: [
    CookieService,
    { provide: NZ_I18N, useValue: en_US }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
