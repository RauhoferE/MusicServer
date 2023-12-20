import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginComponent } from './views/user/login/login.component';
import { RouterModule, Routes } from '@angular/router';
import { RegisterComponent } from './views/user/register/register.component';
import { isAuthenticatedGuard } from './route-guards/is-authenticated.guard';
import { NotFoundComponent } from './views/common/not-found/not-found.component';
import { isUnknownGuard } from './route-guards/is-unknown.guard';
import { ConfirmMailComponent } from './views/user/confirm-mail/confirm-mail.component';
import { ResetPasswordComponent } from './views/user/reset-password/reset-password.component';
import { ForgetPasswordComponent } from './views/user/forget-password/forget-password.component';
import { HomeComponent } from './views/home/home.component';
import { BaseComponent } from './views/base/base.component';
import { PlaylistOverviewComponent } from './views/playlist/playlist-overview/playlist-overview.component';
import { PlaylistDetailsComponent } from './views/playlist/playlist-details/playlist-details.component';
import { FavoritesComponent } from './views/favorites/favorites.component';
import { SongQueueComponent } from './views/playlist/song-queue/song-queue.component';

const routes: Routes = [
  { 
    path: 'login',
    component: LoginComponent,
    canActivate: [isUnknownGuard]
  },
  { 
    path: 'register',
    component: RegisterComponent,
    canActivate: [isUnknownGuard]
  },
  { 
    path: 'confirm/email/:email/:token',
    component: ConfirmMailComponent,
    canActivate: [isUnknownGuard]
  },
  { 
    path: 'reset/password/:userId/:token',
    component: ResetPasswordComponent,
    canActivate: [isUnknownGuard]
  },
  { 
    path: 'forget/password',
    component: ForgetPasswordComponent,
    canActivate: [isUnknownGuard]
  },
  { 
    path: '',
    component: BaseComponent,
    canActivate: [isAuthenticatedGuard],
    children:[
      {
        path: '',
        component: HomeComponent
      },
      {
        path: 'queue',
        component: SongQueueComponent
      },
      {
        path: 'favorites',
        component: FavoritesComponent
      },
      {
        path: 'playlists',
        component: PlaylistOverviewComponent
      },
      {
        path: 'playlist/:playlistId',
        component: PlaylistDetailsComponent
      }
    ]
    //pathMatch: 'full'
  },
  { 
    path: '**',
    component: NotFoundComponent
  },
];

@NgModule({
  declarations: [],
  imports: [
    RouterModule.forRoot(routes)
  ],
  exports: [RouterModule]
})
export class AppRoutingModule { }
