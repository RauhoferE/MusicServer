// Angular Imports
import { NgModule } from '@angular/core';
import en from '@angular/common/locales/en';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { DragDropModule } from '@angular/cdk/drag-drop';
import {ScrollingModule} from '@angular/cdk/scrolling';

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
import { PlaylistDetailsComponent } from './views/playlist/playlist-details/playlist-details.component';
import { SongTableComponent } from './components/songs/song-table/song-table.component';
import { FavoritesComponent } from './views/favorites/favorites.component';
import { SecondsToMinutePipe } from './pipes/seconds-to-minutes-pipe';
import { MediaplayerComponent } from './components/songs/mediaplayer/mediaplayer.component';
import { AudioSvgComponent } from './components/svg/audio-svg/audio-svg.component';
import { AudioOffSvgComponent } from './components/svg/audio-off-svg/audio-off-svg.component';
import { SongQueueComponent } from './views/playlist/song-queue/song-queue.component';
import { AlbumDetailsComponent } from './views/album/album-details/album-details.component';
import { SongDetailsComponent } from './views/song/song-details/song-details.component';
import { AudioLoopComponent } from './components/svg/audio-loop/audio-loop.component';
import { AudioLoopOneComponent } from './components/svg/audio-loop-one/audio-loop-one.component';
import { ArtistDetailsComponent } from './views/artist/artist-details/artist-details.component';
import { AlbumListComponent } from './components/album/album-list/album-list.component';
import { QueueTableComponent } from './components/songs/queue-table/queue-table.component';
import { PlaylistListComponent } from './components/playlist/playlist-list/playlist-list.component';
import { UserDetailsComponent } from './views/user/user-details/user-details.component';
import { PlaylistEditComponent } from './components/playlist/playlist-edit/playlist-edit.component';
import { StreamingClientService } from './services/streaming-client.service';
import { QueueWrapperService } from './services/queue-wrapper.service';
import { RxjsStorageService } from './services/rxjs-storage.service';
import { SessionComponent } from './views/playlist/session/session.component';




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
    PlaylistDetailsComponent,
    SongTableComponent,
    FavoritesComponent,
    SecondsToMinutePipe,
    MediaplayerComponent,
    AudioSvgComponent,
    AudioOffSvgComponent,
    SongQueueComponent,
    AlbumDetailsComponent,
    SongDetailsComponent,
    AudioLoopComponent,
    AudioLoopOneComponent,
    ArtistDetailsComponent,
    AlbumListComponent,
    QueueTableComponent,
    PlaylistListComponent,
    UserDetailsComponent,
    PlaylistEditComponent,
    SessionComponent
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
    ScrollingModule,

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
    { provide: NZ_I18N, useValue: en_US },
    StreamingClientService,
    QueueWrapperService,
    RxjsStorageService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
