"use strict";

function createEntranceTotemLoginPage(){

    return `

        <div class="card my-4 p-3 shadow-lg backLog col-md-5">
          <div class="card-header text-center">
              <h5 class="myText">Login</h5>
          </div>
          <div class="card-body">
              <form id="login-form" name="login-form" method="post">
                  <input type="hidden" id="login-form-user-type" value="UTENTE">
                  <div class="form-group">
                      <label for="login-form-email">Email</label>
                      <input type="email" class="form-control backInput" id="login-form-email" placeholder="Inserisci la tua email">
                      <div id="usernameError" class="invalid-feedback"></div>
                  </div>
                  <div class="form-group">
                      <label for="login-form-password">Password</label>
                      <input type="password" class="form-control backInput" id="login-form-password" minlength="8" placeholder="Inserisci la tua password" pattern="^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$">
                      <div id="passwordError" class="invalid-feedback"></div>
                  </div>                
              </form>
          </div>
          <div class="card-footer d-flex justify-content-end">
              <button type="submit" id="loginBtnEntranceTotem" class="btn myText myButtons">Login</button>
          </div>
        </div>
    `;
}