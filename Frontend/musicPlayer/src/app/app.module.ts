// Angular Imports
import { NgModule } from '@angular/core';
import en from '@angular/common/locales/en';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { DragDropModule } from '@angular/cdk/drag-drop';

// NG-Zorro Imports
import { NZ_I18N } from 'ng-zorro-antd/i18n';
import { en_US } from 'ng-zorro-antd/i18n';
import {NzMessageModule} from 'ng-zorro-antd/message';
import {NzInputModule } from 'ng-zorro-antd/input';
import {NzFormModule} from 'ng-zorro-antd/form';
import { NzIconModule } from 'ng-zorro-antd/icon';
import {NzButtonModule} from 'ng-zorro-antd/button';
import { NzGridModule } from 'ng-zorro-antd/grid';
import {NzDatePickerModule} from 'ng-zorro-antd/date-picker';
import {NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzMenuModule } from 'ng-zorro-antd/menu';
import { NzTagModule } from 'ng-zorro-antd/tag';
import {NzAvatarModule } from 'ng-zorro-antd/avatar';
import {NzListModule } from 'ng-zorro-antd/list';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzDropDownModule } from 'ng-zorro-antd/dropdown';
import {NzSliderModule } from 'ng-zorro-antd/slider';
import {NzPopoverModule } from 'ng-zorro-antd/popover';
import { NzToolTipModule } from 'ng-zorro-antd/tooltip';

// Project Imports
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './views/user/login/login.component';
import { RegisterComponent } from './views/user/register/register.component';
import { CommonModule, registerLocaleData } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';
import { NotFoundComponent } from './views/common/not-found/not-found.component';
import { ConfirmMailComponent } from './views/user/confirm-mail/confirm-mail.component';
import { ResetPasswordComponent } from './views/user/reset-password/reset-password.component';
import { ForgetPasswordComponent } from './views/user/forget-password/forget-password.component';
import { BaseComponent } from './views/base/base.component';
import { HomeComponent } from './views/home/home.component';
import { PlaylistOverviewComponent } from './views/playlist/playlist-overview/playlist-overview.component';
import { PlaylistDetailsComponent } from './views/playlist/playlist-details/playlist-details.component';
import { SongTableComponent } from './components/songs/song-table/song-table.component';
import { FavoritesComponent } from './views/favorites/favorites.component';
import { SecondsToMinutePipe } from './pipes/seconds-to-minutes-pipe';
import { MediaplayerComponent } from './components/songs/mediaplayer/mediaplayer.component';
import { AudioSvgComponent } from './components/svg/audio-svg/audio-svg.component';
import { AudioOffSvgComponent } from './components/svg/audio-off-svg/audio-off-svg.component';
import { SongQueueComponent } from './views/playlist/song-queue/song-queue.component';




registerLocaleData(en);

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegisterComponent,
    NotFoundComponent,
    ConfirmMailComponent,
    ResetPasswordComponent,
    ForgetPasswordComponent,
    BaseComponent,
    HomeComponent,
    PlaylistOverviewComponent,
    PlaylistDetailsComponent,
    SongTableComponent,
    FavoritesComponent,
    SecondsToMinutePipe,
    MediaplayerComponent,
    AudioSvgComponent,
    AudioOffSvgComponent,
    SongQueueComponent
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
    DragDropModule,

    // Ng-zorro
    NzMessageModule,
    NzInputModule,
    NzFormModule,
    NzIconModule,
    NzButtonModule,
    NzGridModule,
    NzDatePickerModule,
    NzLayoutModule,
    NzMenuModule,
    NzTagModule,
    NzAvatarModule,
    NzListModule,
    NzTableModule,
    NzModalModule,
    NzDropDownModule,
    NzSliderModule ,
    NzPopoverModule,
    NzToolTipModule
  ],
  providers: [
    CookieService,
    { provide: NZ_I18N, useValue: en_US }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
