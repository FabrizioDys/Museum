//
// Copyright (c) 2016-2019 Vinnie Falco (vinnie dot falco at gmail dot com)
//
// Distributed under the Boost Software License, Version 1.0. (See accompanying
// file LICENSE_1_0.txt or copy at http://www.boost.org/LICENSE_1_0.txt)
//
// Official repository: https://github.com/boostorg/beast
//

//------------------------------------------------------------------------------
//
// Useful arguments for downloading the right data:
//
//------------------------------------------------------------------------------

#include "root_certificates.hpp"

#include <boost/beast/core.hpp>
#include <boost/beast/http.hpp>
#include <boost/beast/ssl.hpp>
#include <boost/beast/version.hpp>
#include <boost/asio/strand.hpp>
#include <boost/beast/http/error.hpp>
#include <cstdlib>
#include <functional>
#include <iostream>
#include <memory>
#include <string>
#include <fstream>
#include <tchar.h>

#define BUFSIZE 2600000

DWORD WINAPI InstanceThread(LPVOID);

namespace beast = boost::beast;         // from <boost/beast.hpp>
namespace http = beast::http;           // from <boost/beast/http.hpp>
namespace net = boost::asio;            // from <boost/asio.hpp>
namespace ssl = boost::asio::ssl;       // from <boost/asio/ssl.hpp>
using tcp = boost::asio::ip::tcp;       // from <boost/asio/ip/tcp.hpp>
std::string a;
int bytesToReply;
bool isFirst = false;
bool isSecond = false;
bool isThird = false;
bool isFourth = false;
bool isFifth = false;
std::string suffPipe;
int dimOfLODs[50] = {0}; // support up to 50 Levels of Detail
int sumsOfDims[50] = {0}; // values of the increasing size of payload
int nOfLODs;



//------------------------------------------------------------------------------

// Report a failure
void
fail(beast::error_code ec, char const* what)
{
    std::cerr << what << ": " << ec.message() << "\n";
}


int Pipis()
{
  BOOL fConnected = FALSE;
  DWORD dwThreadId = 0;
  HANDLE hPipe = INVALID_HANDLE_VALUE, hThread = NULL;
  std::string nameOfPipe = "\\\\.\\pipe\\mynamedpipe";
  nameOfPipe += suffPipe;
  LPCTSTR lpszPipename = TEXT(nameOfPipe.c_str());
  int nConnections = 0;

  for(;;)
  {


    _tprintf(TEXT("\nPipe Server: Main thread awaiting client connection "
                  "on %s\n"),
             lpszPipename);
    hPipe = CreateNamedPipe(lpszPipename,             // pipe name
                            PIPE_ACCESS_DUPLEX,       // read/write access
                            PIPE_TYPE_BYTE |          // byte type pipe
                                PIPE_READMODE_BYTE |  // byte-read mode
                                PIPE_WAIT,            // blocking mode
                            PIPE_UNLIMITED_INSTANCES, // max. instances
                            BUFSIZE,                  // output buffer size
                            BUFSIZE,                  // input buffer size
                            0,                        // client time-out
                            NULL); // default security attribute

    if(hPipe == INVALID_HANDLE_VALUE)
    {
      _tprintf(TEXT("CreateNamedPipe failed, GLE=%d.\n"), GetLastError());
      return -1;
    }

    // Wait for the client to connect; if it succeeds,
    // the function returns a nonzero value. If the function
    // returns zero, GetLastError returns ERROR_PIPE_CONNECTED.

    fConnected = ConnectNamedPipe(hPipe, NULL)
                     ? TRUE
                     : (GetLastError() == ERROR_PIPE_CONNECTED);

    if(fConnected)
    {
      printf("Client connected, creating a processing thread.\n");
      nConnections++;

      // Create a thread for this client.
      hThread = CreateThread(NULL,           // no security attribute
                             0,              // default stack size
                             InstanceThread, // thread proc
                             (LPVOID)hPipe,  // thread parameter
                             0,            // not suspended
                             &dwThreadId); // returns thread ID

      if(hThread == NULL)
      {
        _tprintf(TEXT("CreateThread failed, GLE=%d.\n"), GetLastError());
        return -1;
      }
      else
      {
        // CloseHandle(hThread);
      }
    }
    else
    { // The client could not connect, so close the pipe.
      CloseHandle(hPipe);
    }

    WaitForSingleObject(hThread, INFINITE);
    return 0;
  }
}



// Performs an HTTP GET and prints the response
class session : public std::enable_shared_from_this<session>
{
    tcp::resolver resolver_;
    beast::ssl_stream<beast::tcp_stream> stream_;
    beast::flat_buffer buffer_; // (Must persist between reads)
    http::request<http::empty_body> req_;
    http::response<http::string_body> res_;
    boost::beast::http::response_parser< boost::beast::http::string_body >  parser_;

public:
    explicit
    session(
        net::executor ex,
        ssl::context& ctx)
    : resolver_(ex)
    , stream_(ex, ctx)
    {
    }

    // Start the asynchronous operation
    void
    run(
        char const* host,
        char const* port,
        char const* target,
        int version)
    {
        // Set SNI Hostname (many hosts need this to handshake successfully)
        if(! SSL_set_tlsext_host_name(stream_.native_handle(), host))
        {
            beast::error_code ec{static_cast<int>(::ERR_get_error()), net::error::get_ssl_category()};
            std::cerr << ec.message() << "\n";
            return;
        }

        // Set up an HTTP GET request message
        req_.version(version);
        req_.method(http::verb::get);
        req_.target(target);
        req_.set(http::field::host, host);
        req_.set(http::field::user_agent, BOOST_BEAST_VERSION_STRING);
        

        // Look up the domain name
        resolver_.async_resolve(
            host,
            port,
            beast::bind_front_handler(
                &session::on_resolve,
                shared_from_this()));
    }

    void
    on_resolve(
        beast::error_code ec,
        tcp::resolver::results_type results)
    {
        if(ec)
            return fail(ec, "resolve");

        // Set a timeout on the operation
        beast::get_lowest_layer(stream_).expires_after(std::chrono::seconds(30));

        // Make the connection on the IP address we get from a lookup
        beast::get_lowest_layer(stream_).async_connect(
            results,
            beast::bind_front_handler(
                &session::on_connect,
                shared_from_this()));
    }

    void
    on_connect(beast::error_code ec, tcp::resolver::results_type::endpoint_type)
    {
        if(ec)
            return fail(ec, "connect");

        // Perform the SSL handshake
        stream_.async_handshake(
            ssl::stream_base::client,
            beast::bind_front_handler(
                &session::on_handshake,
                shared_from_this()));
    }

    void
    on_handshake(beast::error_code ec)
    {
        if(ec)
            return fail(ec, "handshake");

        // Set a timeout on the operation
        beast::get_lowest_layer(stream_).expires_after(std::chrono::seconds(30));

        // Send the HTTP request to the remote host
        http::async_write(stream_, req_,
            beast::bind_front_handler(
                &session::on_write,
                shared_from_this()));
    }

    void
    on_write(
        beast::error_code ec,
        std::size_t bytes_transferred)
    {
        boost::ignore_unused(bytes_transferred);

        if(ec)
            return fail(ec, "write");

        //In this moment in res_ there is the 200 OK from HTTP

       
        // Receive the HTTP response
        http::async_read_some(
            stream_,
            buffer_,
            parser_,
            beast::bind_front_handler(&session::on_read, shared_from_this()));
           
    }

    int s = 0; //per i file!!

    void
    on_read(
        beast::error_code ec,
        std::size_t bytes_transferred)
    {
        boost::ignore_unused(bytes_transferred);


         if(ec)
          return fail(ec, "read");

        if(!parser_.is_done())
        {
          a = parser_.get().body();

          if(a.size() > dimOfLODs[0] && !isFirst)
            {
              isFirst = true; //changeable with an array of bool 
              bytesToReply = dimOfLODs[0];
                //trimma togliendo il primo pezzo che non ha senso inviarlo
              Pipis();
            }
          if(a.size() > sumsOfDims[1] && !isSecond) // do a loop from lod(1) to lod(n-1) to ensure portability
            {   
              isSecond = true;
              std::string temp(a.begin() + dimOfLODs[0], a.end() - 1);
              a = temp;
             
              // trimma togliendo il primo pezzo che non ha senso inviarlo
              bytesToReply = dimOfLODs[1];
              Pipis();
            }
            if(a.size() > sumsOfDims[2] && !isThird)
            {
              isThird = true;
              std::string temp(a.begin() + sumsOfDims[1], a.end() - 1);
              a = temp;
              // trimma togliendo il primo pezzo che non ha senso inviarlo
              bytesToReply = dimOfLODs[2];
              Pipis();
            }
            if(a.size() > sumsOfDims[3] && !isFourth)
            {
              isFourth = true;
              std::string temp(a.begin() + sumsOfDims[2], a.end() - 1);
              a = temp;
              // trimma togliendo il primo pezzo che non ha senso inviarlo
              bytesToReply = dimOfLODs[3];
              Pipis();
            }
            if(a.size() > sumsOfDims[4] && !isFifth)
            {
              isFifth = true;
              std::string temp(a.begin() + sumsOfDims[3], a.end() - 1);
              a = temp;
              // trimma togliendo il primo pezzo che non ha senso inviarlo
              bytesToReply = dimOfLODs[4];
              Pipis();
            }
          
          s++;

          http::async_read_some(
              stream_,
              buffer_,
              parser_,
              beast::bind_front_handler(&session::on_read, shared_from_this()));
        }
        else
        {
          a = parser_.get().body();

          // Write the message to the final file
        /*  std::string value = "C:/Users/fabro/MEPP2/build/Visualization/Applications/Release/Codecs/prova" +std::to_string(s);
          std::string final = ".p3d";
          std::ofstream outpoot(value + final, std::ofstream::binary);
          outpoot << parser_.get().body();
          outpoot.close();*/

          std::string temp(a.begin() + sumsOfDims[4], a.end() - 1);
          a = temp;
          bytesToReply = dimOfLODs[5];
           Pipis();

          // Set a timeout on the operation
          beast::get_lowest_layer(stream_).expires_after(
              std::chrono::seconds(30));

          // Gracefully close the stream
          stream_.async_shutdown(beast::bind_front_handler(
              &session::on_shutdown, shared_from_this()));
        }
    }

    void
    on_shutdown(beast::error_code ec)
    {
        if(ec == net::error::eof)
        {
            // Rationale:
            // http://stackoverflow.com/questions/25587403/boost-asio-ssl-async-shutdown-always-finishes-with-an-error
            ec = {};
        }
        if(ec)
            return fail(ec, "shutdown");

        // If we get here then the connection is closed gracefully
    }
};

//------------------------------------------------------------------------------


int main(int argc, char** argv)
{
  try
  {


    // Check command line arguments.
    if(argc != 5 && argc != 6)
    {
      std::cerr << "Usage: http-client-async-ssl <host> <port> <target> [<HTTP "
                   "version: 1.0 or 1.1(default)>]\n"
                << "Example:\n"
                << "    http-client-async-ssl www.example.com 443 / suffixOfPipe \n"
                << "    http-client-async-ssl www.example.com 443 / suffixOfPipe 1.0\n";
      return EXIT_FAILURE;
    }
    auto const host = argv[1];
    auto const port = argv[2];
    auto const target = argv[3];
    auto const suffixPipe = argv[4];
    int version = argc == 6 && !std::strcmp("1.0", argv[5]) ? 10 : 11;

    suffPipe = suffixPipe;

    if(suffPipe == "Horse")
    {
      nOfLODs = 6;

      dimOfLODs[0] = 7695;
      dimOfLODs[1] = 49586;
      dimOfLODs[2] = 121190;
      dimOfLODs[3] = 330208;
      dimOfLODs[4] = 822196;
      dimOfLODs[5] = 1475130;
    }
    if(suffPipe == "Lion")
    {
      nOfLODs = 6;

      dimOfLODs[0] = 7699;
      dimOfLODs[1] = 55526;
      dimOfLODs[2] = 213994;
      dimOfLODs[3] = 380818;
      dimOfLODs[4] = 1155600;
      dimOfLODs[5] = 1743345;
    }
    if(suffPipe == "Cat")
    {
      nOfLODs = 6;

      dimOfLODs[0] = 5276;
      dimOfLODs[1] = 33548;
      dimOfLODs[2] = 135239;
      dimOfLODs[3] = 249333;
      dimOfLODs[4] = 1078505;
      dimOfLODs[5] = 2366783;
    }
    if(suffPipe == "Putti")
    {
      nOfLODs = 6;

      dimOfLODs[0] = 7830;
      dimOfLODs[1] = 51066;
      dimOfLODs[2] = 125274;
      dimOfLODs[3] = 342107;
      dimOfLODs[4] = 847620;
      dimOfLODs[5] = 1554122;
    }
    if(suffPipe == "Sphinx")
    {
      nOfLODs = 6;

      dimOfLODs[0] = 7906;
      dimOfLODs[1] = 52090;
      dimOfLODs[2] = 125986;
      dimOfLODs[3] = 344654;
      dimOfLODs[4] = 847968;
      dimOfLODs[5] = 1549682;
    }

    for(int i = 0; i < nOfLODs; i++)
    {
      for(int j = 0; j <= i; j++)
      {
        sumsOfDims[i] += dimOfLODs[j];
      }
    }

    // The io_context is required for all I/O
    net::io_context ioc;

    // The SSL context is required, and holds certificates
    ssl::context ctx{ssl::context::tlsv12_client};

    // This holds the root certificate used for verification
    load_root_certificates(ctx);

    // Verify the remote server's certificate
    // ctx.set_verify_mode(ssl::verify_peer);


    // Launch the thread that reads
    // Launch the asynchronous operation
    // The session is constructed with a strand to
    // ensure that handlers do not execute concurrently.
    std::make_shared< session >(net::make_strand(ioc), ctx)
        ->run(host, port, target, version);


    // Run the I/O service. The call will return when
    // the get operation is complete.
    ioc.run();

  }
  catch(std::exception const &e)
  {
    std::cerr << "Error: " << e.what() << std::endl;
    return EXIT_FAILURE;
  }
  return EXIT_SUCCESS;
}





DWORD WINAPI
InstanceThread(LPVOID lpvParam)
// This routine is a thread processing function to read from and reply to a
// client via the open pipe connection passed from the main loop. Note this
// allows the main loop to continue executing, potentially creating more threads
// of of this procedure to run concurrently, depending on the number of incoming
// client connections.
{
  HANDLE hHeap = GetProcessHeap();
  TCHAR *pchRequest = (TCHAR *)HeapAlloc(hHeap, 0, BUFSIZE * sizeof(TCHAR));
  TCHAR *pchReply = (TCHAR *)HeapAlloc(hHeap, 0, BUFSIZE * sizeof(TCHAR));


  DWORD cbBytesRead = 0, cbReplyBytes = 0, cbWritten = 0;
  BOOL fSuccess = FALSE;
  HANDLE hPipe = NULL;

  // Do some extra error checking since the app will keep running even if this
  // thread fails.

  if(lpvParam == NULL)
  {
    printf("\nERROR - Pipe Server Failure:\n");
    printf("   InstanceThread got an unexpected NULL value in lpvParam.\n");
    printf("   InstanceThread exitting.\n");
    if(pchReply != NULL)
      HeapFree(hHeap, 0, pchReply);
    if(pchRequest != NULL)
      HeapFree(hHeap, 0, pchRequest);
    return (DWORD)-1;
  }

  if(pchRequest == NULL)
  {
    printf("\nERROR - Pipe Server Failure:\n");
    printf("   InstanceThread got an unexpected NULL heap allocation.\n");
    printf("   InstanceThread exitting.\n");
    if(pchReply != NULL)
      HeapFree(hHeap, 0, pchReply);
    return (DWORD)-1;
  }

  if(pchReply == NULL)
  {
    printf("\nERROR - Pipe Server Failure:\n");
    printf("   InstanceThread got an unexpected NULL heap allocation.\n");
    printf("   InstanceThread exitting.\n");
    if(pchRequest != NULL)
      HeapFree(hHeap, 0, pchRequest);
    return (DWORD)-1;
  }

  // Print verbose messages. In production code, this should be for debugging
  // only.
  printf("InstanceThread created, receiving and processing messages.\n");

  // The thread's parameter is a handle to a pipe object instance.

  hPipe = (HANDLE)lpvParam;

  // Loop until done reading
  while(1)
  {
    // Read client requests from the pipe. This simplistic code only allows
    // messages up to BUFSIZE characters in length.
    fSuccess = ReadFile(hPipe,                   // handle to pipe
                        pchRequest,              // buffer to receive data
                        BUFSIZE * sizeof(TCHAR), // size of buffer
                        &cbBytesRead,            // number of bytes read
                        NULL);                   // not overlapped I/O

    if(!fSuccess || cbBytesRead == 0)
    {
      if(GetLastError() == ERROR_BROKEN_PIPE)
      {
        _tprintf(TEXT("InstanceThread: client disconnected.\n"));
      }
      else
      {
        _tprintf(TEXT("InstanceThread ReadFile failed, GLE=%d.\n"),
                 GetLastError());
      }
      break;
    }

       int temp = *pchRequest;
   
       std::cout << temp << "\n";

    if(temp == 48 ) //0 on c# side is 48 here, use it to discriminate meshes
    {
    
      _tprintf(TEXT("Yeah, it was number zero, Borderlands Cosplay.\n"));
    }


    // Process the incoming message.
    // GetAnswerToRequest(pchRequest, pchReply, &cbReplyBytes);
    //  printf("Continuing..\n"); // qua ci arriva


    // my part2
    /*
    std::ifstream uncompressedFile;
    uncompressedFile.open("C:/Users/fabro/MEPP2/build/Visualization/"
                          "Applications/Release/Codecs/prova811.p3d", //need to find a way to access the buffer, parameter?
                          std::ifstream::binary);
    std::streambuf *raw = uncompressedFile.rdbuf(); // see if any streambuf member gives back size of data



    uncompressedFile.ignore(std::numeric_limits< std::streamsize >::max());
    std::streamsize lengthOfFile = uncompressedFile.gcount();
    uncompressedFile.clear(); //  Since ignore will have set eof.
    uncompressedFile.seekg(0, std::ios_base::beg);

    */

    cbReplyBytes = bytesToReply;
    

    //cbReplyBytes = (lengthOfFile) * sizeof(TCHAR);
    const char *contents  = new char[cbReplyBytes]; // trova la dimensione corretta
    contents = a.c_str();
    
    //raw->sgetn(contents, cbReplyBytes);



    // Write the reply to the pipe.
    fSuccess = WriteFile(
        hPipe,        // handle to pipe
        contents,     // buffer to write from , I wrote raw instead of pchReply
        cbReplyBytes, // number of bytes to write
        &cbWritten,   // number of bytes written
        NULL);        // not overlapped I/O

    // memset(pchReply, 0xCC, BUFSIZE);

    if(!fSuccess || cbReplyBytes != cbWritten)
    {
      _tprintf(TEXT("InstanceThread WriteFile failed, GLE=%d.\n"),
               GetLastError());
      break;
    }
    //uncompressedFile.close();
  }

  // Flush the pipe to allow the client to read the pipe's contents
  // before disconnecting. Then disconnect the pipe, and close the
  // handle to this pipe instance.

  FlushFileBuffers(hPipe);
  DisconnectNamedPipe(hPipe);
  CloseHandle(hPipe);

  HeapFree(hHeap, 0, pchRequest);
  HeapFree(hHeap, 0, pchReply);

  printf("InstanceThread exiting.\n");
  return 1;
}
